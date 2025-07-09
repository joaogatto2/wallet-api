using System.Linq.Expressions;
using Moq;
using Wallet.Application.Services;
using Wallet.Core.DTOs;
using Wallet.Core.Entities;
using Wallet.Core.Repositories;
using Wallet.Core.Services;
using Xunit;

namespace Wallet.Tests.Application.Services;

public class TransferServiceTests
{
    private readonly Mock<IBaseRepository<Transfer>> _transferRepoMock;
    private readonly Mock<IBaseRepository<User>> _userRepoMock;
    private readonly Mock<ISignedInUserService> _signedInUserMock;
    private readonly TransferService _service;

    public TransferServiceTests()
    {
        _transferRepoMock = new Mock<IBaseRepository<Transfer>>();
        _userRepoMock = new Mock<IBaseRepository<User>>();
        _signedInUserMock = new Mock<ISignedInUserService>();
        _signedInUserMock.SetupGet(x => x.UserId).Returns(1);
        _service = new TransferService(_transferRepoMock.Object, _userRepoMock.Object, _signedInUserMock.Object);
    }

    [Fact]
    public async Task Create_Throws_When_Transferring_To_Self()
    {
        var req = new CreateTransferRequest { ToUserId = 1, Value = 10 };
        await Assert.ThrowsAsync<Exception>(() => _service.Create(req));
    }

    [Fact]
    public async Task Create_Throws_When_Insufficient_Balance()
    {
        var req = new CreateTransferRequest { ToUserId = 2, Value = 100 };
        _userRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new User { Id = 1, Balance = 50 });
        await Assert.ThrowsAsync<Exception>(() => _service.Create(req));
    }

    [Fact]
    public async Task Create_Throws_When_ToUser_Not_Found()
    {
        var req = new CreateTransferRequest { ToUserId = 2, Value = 10 };
        _userRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new User { Id = 1, Balance = 100 });
        _userRepoMock.Setup(x => x.GetByIdAsync(2)).ReturnsAsync((User)null);
        await Assert.ThrowsAsync<Exception>(() => _service.Create(req));
    }

    [Fact]
    public async Task Create_Succeeds_And_Updates_Balances()
    {
        var req = new CreateTransferRequest { ToUserId = 2, Value = 10 };
        var fromUser = new User { Id = 1, Balance = 100 };
        var toUser = new User { Id = 2, Balance = 20 };
        _userRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(fromUser);
        _userRepoMock.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(toUser);

        await _service.Create(req);

        Assert.Equal(90, fromUser.Balance);
        Assert.Equal(30, toUser.Balance);
        _transferRepoMock.Verify(x => x.AddAsync(It.IsAny<Transfer>()), Times.Once);
        _userRepoMock.Verify(x => x.Update(fromUser), Times.Once);
        _userRepoMock.Verify(x => x.Update(toUser), Times.Once);
        _transferRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Get_Returns_Correct_Transfers()
    {
        var req = new GetTransfersRequest();
        var inTransfers = new List<Transfer> { new Transfer { Id = 1, ToUserId = 1 } };
        var outTransfers = new List<Transfer> { new Transfer { Id = 2, ByUserId = 1 } };

        _transferRepoMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Transfer, bool>>>()))
            .ReturnsAsync((Expression<Func<Transfer, bool>> pred) =>
            {
                if (pred.Compile().Invoke(new Transfer { ToUserId = 1 }))
                    return inTransfers;
                if (pred.Compile().Invoke(new Transfer { ByUserId = 1 }))
                    return outTransfers;
                return new List<Transfer>();
            });

        var result = await _service.Get(req);

        Assert.Equal(inTransfers, result.In);
        Assert.Equal(outTransfers, result.Out);
    }
}