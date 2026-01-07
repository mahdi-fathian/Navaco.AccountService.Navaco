namespace Navaco.AccountService.Domain.Exceptions;

public class CurrencyMismatchException : DomainException
{
    public string LeftCurrency { get; }
    public string RightCurrency { get; }

    public CurrencyMismatchException(string leftCurrency, string rightCurrency)
        : base($"عدم تطابق ارز: {leftCurrency} و {rightCurrency}")
    {
        LeftCurrency = leftCurrency;
        RightCurrency = rightCurrency;
    }
}
