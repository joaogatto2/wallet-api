using Wallet.Core.DTOs;
using Wallet.Core.Entities;
using Wallet.Core.Repositories;
using Wallet.Core.Services;

namespace Wallet.Application.Services;

public class TransferService(IBaseRepository<Transfer> transferRepo, IBaseRepository<User> userRepo, ISignedInUserService signedInUser) : ITransferService
{
    public async Task Create(CreateTransferRequest request)
    {
        if (request.ToUserId == signedInUser.UserId)
            throw new Exception("Cannot transfer to the same user");
            
        var currentUser = await userRepo.GetByIdAsync(signedInUser.UserId);

        if (request.Value > currentUser.Balance)
            throw new Exception("Insufficient balance");

        var toUser = await userRepo.GetByIdAsync(request.ToUserId);

        if (toUser == null)
            throw new Exception("User not found");

        var transfer = new Transfer
        {
            Date = DateTime.UtcNow,
            ByUserId = (int)signedInUser.UserId,
            ToUserId = request.ToUserId,
            Value = request.Value,
        };

        currentUser.Balance -= request.Value;
        toUser.Balance += request.Value;

        await transferRepo.AddAsync(transfer);
        userRepo.Update(currentUser);
        userRepo.Update(toUser);

        await transferRepo.SaveChangesAsync();
    }

    public async Task<GetTransfersResponse> Get(GetTransfersRequest request)
    {
        var transfersIn = await transferRepo.FindAsync(x => 
            x.ToUserId == signedInUser.UserId &&
            (request.StartDate == null || x.Date >= request.StartDate.Value.ToUniversalTime()) &&
            (request.EndDate == null || x.Date <= request.EndDate.Value.ToUniversalTime())
        );
        var transfersOut = await transferRepo.FindAsync(x => 
            x.ByUserId == signedInUser.UserId &&
            (request.StartDate == null || x.Date >= request.StartDate.Value.ToUniversalTime()) &&
            (request.EndDate == null || x.Date <= request.EndDate.Value.ToUniversalTime())
        );

        return new GetTransfersResponse { In = transfersIn, Out = transfersOut };
    }
}