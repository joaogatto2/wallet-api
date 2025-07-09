using Wallet.Core.Entities;
using Wallet.Core.Repositories;
using Wallet.Core.Services;

namespace Wallet.Application.Services;

public class UserService(IBaseRepository<User> userRepo, ISignedInUserService signedInUser) : IUserService
{
    public async Task<User> Create(string name)
    {
        var user = new User
        {
            Name = name,
            Balance = 0
        };

        await userRepo.AddAsync(user);
        await userRepo.SaveChangesAsync();

        return user;
    }

    public async Task<decimal> GetBalance()
    {
        var user = await userRepo.GetByIdAsync(signedInUser.UserId);

        return user!.Balance;
    }
}