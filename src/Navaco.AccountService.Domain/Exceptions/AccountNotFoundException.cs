namespace Navaco.AccountService.Domain.Exceptions;

public class AccountNotFoundException : DomainException
{
    public Guid AccountId { get; }

    public AccountNotFoundException(Guid accountId)
        : base($"حساب با شناسه {accountId} یافت نشد.")
    {
        AccountId = accountId;
    }
}
