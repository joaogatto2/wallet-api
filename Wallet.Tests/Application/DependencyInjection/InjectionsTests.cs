using Microsoft.Extensions.DependencyInjection;
using Wallet.Application.DependencyInjection;
using Wallet.Infrastructure.DependencyInjection;
using Wallet.Application.Services;
using Wallet.Core.Services;
using Microsoft.Extensions.Configuration;

namespace Wallet.Tests.Application.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Wallet.Application.Services;
using Wallet.Core.Services;
using Wallet.Core.Models;
using Xunit;
using Microsoft.Extensions.Options;

public class InjectionsTests
{
    [Fact]
    public void AddApplicationServices_RegistersAllDependencies()
    {
        // Arrange
        var services = new ServiceCollection();
        var configurationData = new Dictionary<string, string>
        {
            { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=WalletDb;User Id=sa;Password=Your_password123;" },
            { "JwtSettings:Secret", "ThisIsASecretForTesting123456" },
            { "JwtSettings:Issuer", "TestIssuer" },
            { "JwtSettings:Audience", "TestAudience" },
            { "JwtSettings:ExpiryMinutes", "30" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddInfrastructureServices(configuration);
        services.AddJwt(configuration);
        services.AddHttpContextAccessor();

        // Act
        services.AddApplicationServices();
        var provider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(provider.GetService<IAuthService>());
        Assert.NotNull(provider.GetService<ISignedInUserService>());
        Assert.NotNull(provider.GetService<IDepositService>());
        Assert.NotNull(provider.GetService<ITransferService>());
        Assert.NotNull(provider.GetService<IUserService>());
    }

    [Fact]
    public void AddJwt_ConfiguresJwtCorrectly()
    {
        // Arrange
        var configData = new Dictionary<string, string>
        {
            { "JwtSettings:Secret", "ThisIsASecretForTesting123456" },
            { "JwtSettings:Issuer", "TestIssuer" },
            { "JwtSettings:Audience", "TestAudience" },
            { "JwtSettings:ExpiryMinutes", "30" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddJwt(configuration);
        var provider = services.BuildServiceProvider();

        // Assert JwtSettings is bound
        var options = provider.GetService<IOptions<JwtSettings>>();
        Assert.NotNull(options);
        Assert.Equal("ThisIsASecretForTesting123456", options.Value.Secret);
        Assert.Equal("TestIssuer", options.Value.Issuer);
        Assert.Equal("TestAudience", options.Value.Audience);

        // Assert authentication was added
        var authOptions = services.FirstOrDefault(sd =>
            sd.ServiceType.FullName == "Microsoft.AspNetCore.Authentication.IAuthenticationService");
        Assert.NotNull(authOptions);
    }
}
