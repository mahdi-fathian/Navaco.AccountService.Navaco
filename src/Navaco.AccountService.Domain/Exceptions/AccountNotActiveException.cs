namespace Navaco.AccountService.Domain.Exceptions;

public class AccountNotActiveException : DomainException
{
    public Guid AccountId { get; }

    public AccountNotActiveException(Guid accountId)
        : base($"حساب با شناسه {accountId} فعال نیست.")
    {
        AccountId = accountId;
    }
}
