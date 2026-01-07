namespace Navaco.AccountService.Domain.Exceptions;

public class InvalidWithdrawAmountException : DomainException
{
    public decimal AttemptedAmount { get; }

    public InvalidWithdrawAmountException(decimal amount)
        : base($"مبلغ برداشت {amount} نامعتبر است. مبلغ باید بیشتر از صفر باشد.")
    {
        AttemptedAmount = amount;
    }
}
