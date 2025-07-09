using Wallet.Core.Entities;

namespace Wallet.Core.DTOs;

public class GetTransfersResponse
{
    public IEnumerable<Transfer> In { get; set; }
    public IEnumerable<Transfer> Out { get; set; }
}