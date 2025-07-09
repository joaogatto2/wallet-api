using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Wallet.Core.Entities;
using Wallet.Core.Models;
using Wallet.Core.Repositories;
using Wallet.Core.Services;

namespace Wallet.Application.Services;

public class AuthService(IBaseRepository<User> userRepo, IOptions<JwtSettings> jwtSettings) : IAuthService
{
    public async Task<string> GenerateToken(int userId)
    {
        var user = await userRepo.GetByIdAsync(userId);

        if (user == null)
            throw new Exception("User not found");
            
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSettings.Value.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.Value.ExpiryMinutes),
            Issuer = jwtSettings.Value.Issuer,
            Audience = jwtSettings.Value.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}