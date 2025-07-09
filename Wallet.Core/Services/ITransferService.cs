using Wallet.Core.DTOs;
using Wallet.Core.Entities;

namespace Wallet.Core.Services;

public interface ITransferService
{
    Task Create(CreateTransferRequest request);
    Task<GetTransfersResponse> Get(GetTransfersRequest request);
}