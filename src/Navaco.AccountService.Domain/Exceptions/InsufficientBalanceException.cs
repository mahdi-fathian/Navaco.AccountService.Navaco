namespace Navaco.AccountService.Domain.Exceptions;

public class InsufficientBalanceException : DomainException
{
    public decimal CurrentBalance { get; }
    public decimal RequestedAmount { get; }

    public InsufficientBalanceException(decimal currentBalance, decimal requestedAmount)
        : base($"موجودی کافی نیست. موجودی فعلی: {currentBalance}، مبلغ درخواستی: {requestedAmount}")
    {
        CurrentBalance = currentBalance;
        RequestedAmount = requestedAmount;
    }
}
