namespace Wallet.Core.Services;

public interface IAuthService
{
    Task<string> GenerateToken(int userId);  
}