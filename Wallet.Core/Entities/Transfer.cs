namespace Wallet.Core.Entities;

public class Transfer
{
    public int Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public int ByUserId { get; set; }
    public User ByUser { get; set; }
    public int ToUserId { get; set; }
    public User ToUser { get; set; }
    public decimal Value { get; set; }
}