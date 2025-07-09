using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using Wallet.Application.Services;

namespace Wallet.Tests.Application.Services;

public class SignedInUserServiceTests
{
    [Fact]
    public void UserId_ReturnsUserId_WhenClaimExistsAndIsValid()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "123") };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.User).Returns(principal);

        var mockAccessor = new Mock<IHttpContextAccessor>();
        mockAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

        var service = new SignedInUserService(mockAccessor.Object);

        // Act
        var result = service.UserId;

        // Assert
        Assert.Equal(123, result);
    }

    [Fact]
    public void UserId_ReturnsNull_WhenClaimIsMissing()
    {
        // Arrange
        var identity = new ClaimsIdentity(); // no claims
        var principal = new ClaimsPrincipal(identity);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.User).Returns(principal);

        var mockAccessor = new Mock<IHttpContextAccessor>();
        mockAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

        var service = new SignedInUserService(mockAccessor.Object);

        // Act
        var result = service.UserId;

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void UserId_ReturnsNull_WhenClaimIsNotAnInteger()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "abc") };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.User).Returns(principal);

        var mockAccessor = new Mock<IHttpContextAccessor>();
        mockAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

        var service = new SignedInUserService(mockAccessor.Object);

        // Act
        var result = service.UserId;

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void UserId_ReturnsNull_WhenHttpContextIsNull()
    {
        // Arrange
        var mockAccessor = new Mock<IHttpContextAccessor>();
        mockAccessor.Setup(a => a.HttpContext).Returns<HttpContext?>(null);

        var service = new SignedInUserService(mockAccessor.Object);

        // Act
        var result = service.UserId;

        // Assert
        Assert.Null(result);
    }
}