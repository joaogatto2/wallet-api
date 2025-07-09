

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Wallet.Core.Models;
using Wallet.Core.Repositories;
using Wallet.Application.Services;
using Xunit;
using Wallet.Core.Entities;

namespace Wallet.Tests.Application.Services;
public class AuthServiceTests
{
    
    private readonly Mock<IBaseRepository<User>> _mockRepo;
    private readonly IOptions<JwtSettings> _jwtOptions;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockRepo = new Mock<IBaseRepository<User>>();

        var settings = new JwtSettings
        {
            Secret = "MyUltraSecureTestKey1234567890MyUltraSecureTestKey1234567890MyUltraSecureTestKey1234567890MyUltraSecureTestKey1234567890",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = 60
        };

        _jwtOptions = Options.Create(settings);
        _authService = new AuthService(_mockRepo.Object, _jwtOptions);
    }

    [Fact]
    public async Task GenerateToken_ReturnsValidJwt_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId });

        // Act
        var token = await _authService.GenerateToken(userId);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.NotNull(jwtToken);
        Assert.Contains(jwtToken.Claims, c =>
            c.Type == "nameid" && c.Value == userId.ToString());

        Assert.Equal("TestIssuer", jwtToken.Issuer);
        Assert.Equal("TestAudience", jwtToken.Audiences.First());
    }

    [Fact]
    public async Task GenerateToken_ThrowsException_WhenUserNotFound()
    {
        // Arrange
        var userId = 999;
        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _authService.GenerateToken(userId));
        Assert.Equal("User not found", ex.Message);
    }
}
