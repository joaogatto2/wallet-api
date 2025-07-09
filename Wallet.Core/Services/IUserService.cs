using Wallet.Core.Entities;

namespace Wallet.Core.Services;

public interface IUserService
{
    Task<User> Create(string name);
    Task<decimal> GetBalance();
}