namespace Navaco.AccountService.Domain.Exceptions;

public class InvalidDepositAmountException : DomainException
{
    public decimal AttemptedAmount { get; }

    public InvalidDepositAmountException(decimal amount)
        : base($"مبلغ واریز {amount} نامعتبر است. مبلغ باید بیشتر از صفر باشد.")
    {
        AttemptedAmount = amount;
    }
}
