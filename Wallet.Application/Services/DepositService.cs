using Wallet.Core.Entities;
using Wallet.Core.Repositories;
using Wallet.Core.Services;

namespace Wallet.Application.Services;

public class DepositService(IBaseRepository<Deposit> depositRepo, IBaseRepository<User> userRepo, ISignedInUserService signedInUser) : IDepositService
{
    public async Task Deposit(decimal value)
    {
        var user = await userRepo.GetByIdAsync(signedInUser.UserId);
        var deposit = new Deposit
        {
            Date = DateTime.UtcNow,
            UserId = (int)signedInUser.UserId,
            Value = value
        };

        user.Balance += value;
        await depositRepo.AddAsync(deposit);
        userRepo.Update(user);
        await depositRepo.SaveChangesAsync();
    }
}