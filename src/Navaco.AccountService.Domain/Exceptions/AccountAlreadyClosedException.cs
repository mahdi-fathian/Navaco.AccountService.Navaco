namespace Navaco.AccountService.Domain.Exceptions;

public class AccountAlreadyClosedException : DomainException
{
    public Guid AccountId { get; }

    public AccountAlreadyClosedException(Guid accountId)
        : base($"حساب با شناسه {accountId} قبلاً بسته شده است.")
    {
        AccountId = accountId;
    }
}
