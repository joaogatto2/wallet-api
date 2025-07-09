using Moq;
using Wallet.Application.Services;
using Wallet.Core.Entities;
using Wallet.Core.Repositories;
using Wallet.Core.Services;

namespace Wallet.Tests.Application.Services;

public class UserServiceTests
{
    [Fact]
    public async Task Create_ShouldAddUserWithNameAndZeroBalance()
    {
        // Arrange
        var userRepoMock = new Mock<IBaseRepository<User>>();
        userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var signedInUserMock = new Mock<ISignedInUserService>();
        var service = new UserService(userRepoMock.Object, signedInUserMock.Object);

        // Act
        var user = await service.Create("Alice");

        // Assert
        Assert.Equal("Alice", user.Name);
        Assert.Equal(0, user.Balance);
        userRepoMock.Verify(r => r.AddAsync(It.Is<User>(u => u.Name == "Alice" && u.Balance == 0)), Times.Once);
        userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetBalance_ShouldReturnUserBalance()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, Name = "Bob", Balance = 42.5m };

        var userRepoMock = new Mock<IBaseRepository<User>>();
        userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        var signedInUserMock = new Mock<ISignedInUserService>();
        signedInUserMock.Setup(s => s.UserId).Returns(userId);

        var service = new UserService(userRepoMock.Object, signedInUserMock.Object);

        // Act
        var balance = await service.GetBalance();

        // Assert
        Assert.Equal(42.5m, balance);
        userRepoMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
    }
}