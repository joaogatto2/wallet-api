using Moq;
using Wallet.Application.Services;
using Wallet.Core.Entities;
using Wallet.Core.Repositories;
using Wallet.Core.Services;

namespace Wallet.Tests.Application.Services;

public class DepositServiceTests
{
    [Fact]
    public async Task Deposit_ShouldIncreaseUserBalance_AndAddDeposit()
    {
        // Arrange
        var userId = 1;
        var initialBalance = 100m;
        var depositValue = 50m;
        var user = new User { Id = userId, Balance = initialBalance };

        var depositRepoMock = new Mock<IBaseRepository<Deposit>>();
        var userRepoMock = new Mock<IBaseRepository<User>>();
        var signedInUserMock = new Mock<ISignedInUserService>();

        userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        signedInUserMock.Setup(s => s.UserId).Returns(userId);

        var service = new DepositService(depositRepoMock.Object, userRepoMock.Object, signedInUserMock.Object);

        // Act
        await service.Deposit(depositValue);

        // Assert
        Assert.Equal(initialBalance + depositValue, user.Balance);
        depositRepoMock.Verify(r => r.AddAsync(It.Is<Deposit>(d =>
            d.UserId == userId &&
            d.Value == depositValue &&
            d.Date <= DateTime.UtcNow
        )), Times.Once);
        userRepoMock.Verify(r => r.Update(user), Times.Once);
        depositRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Deposit_ShouldThrow_WhenUserNotFound()
    {
        // Arrange
        var userId = 1;
        var depositRepoMock = new Mock<IBaseRepository<Deposit>>();
        var userRepoMock = new Mock<IBaseRepository<User>>();
        var signedInUserMock = new Mock<ISignedInUserService>();

        userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);
        signedInUserMock.Setup(s => s.UserId).Returns(userId);

        var service = new DepositService(depositRepoMock.Object, userRepoMock.Object, signedInUserMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => service.Deposit(10m));
    }

    [Fact]
    public async Task Deposit_ShouldAllowZeroDeposit()
    {
        // Arrange
        var userId = 1;
        var initialBalance = 100m;
        var depositValue = 0m;
        var user = new User { Id = userId, Balance = initialBalance };

        var depositRepoMock = new Mock<IBaseRepository<Deposit>>();
        var userRepoMock = new Mock<IBaseRepository<User>>();
        var signedInUserMock = new Mock<ISignedInUserService>();

        userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        signedInUserMock.Setup(s => s.UserId).Returns(userId);

        var service = new DepositService(depositRepoMock.Object, userRepoMock.Object, signedInUserMock.Object);

        // Act
        await service.Deposit(depositValue);

        // Assert
        Assert.Equal(initialBalance, user.Balance);
        depositRepoMock.Verify(r => r.AddAsync(It.Is<Deposit>(d => d.Value == depositValue)), Times.Once);
        userRepoMock.Verify(r => r.Update(user), Times.Once);
        depositRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}