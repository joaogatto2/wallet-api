namespace Wallet.Core.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }
    public virtual ICollection<Deposit> Deposits { get; set; }
    public virtual ICollection<Transfer> TransfersIn { get; set; }
    public virtual ICollection<Transfer> TransfersOut { get; set; }
}