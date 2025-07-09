namespace Wallet.Core.DTOs;

public class GetTransfersRequest
{
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}