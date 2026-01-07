namespace Navaco.AccountService.Domain.Exceptions;

public class InvalidMoneyAmountException : DomainException
{
    public decimal AttemptedAmount { get; }

    public InvalidMoneyAmountException(decimal amount)
        : base($"مبلغ {amount} نامعتبر است. مبلغ نمی‌تواند منفی باشد.")
    {
        AttemptedAmount = amount;
    }
}
