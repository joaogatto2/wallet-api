using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Wallet.Application.Services;
using Wallet.Core.Models;
using Wallet.Core.Services;

namespace Wallet.Application.DependencyInjection;

public static class Injections
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services) => services
        .AddScoped<IAuthService, AuthService>()
        .AddScoped<ISignedInUserService, SignedInUserService>()
        .AddScoped<IDepositService, DepositService>()
        .AddScoped<ITransferService, TransferService>()
        .AddScoped<IUserService, UserService>();

    public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience
            };
        });

        services.AddAuthorization();
    }
}