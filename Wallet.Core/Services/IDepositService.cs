namespace Wallet.Core.Services;

public interface IDepositService
{
    Task Deposit(decimal value);
}