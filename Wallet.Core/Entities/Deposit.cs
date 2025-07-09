namespace Wallet.Core.Entities;

public class Deposit
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public decimal Value { get; set; }
}