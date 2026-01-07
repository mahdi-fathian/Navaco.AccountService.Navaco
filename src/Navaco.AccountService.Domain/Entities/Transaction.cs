namespace Navaco.AccountService.Domain.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.

public class Transaction : Entity
{
    public Guid AccountId { get; private set; }
    public Money Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Transaction() { } // EF Core

    public Transaction(Guid accountId, Money amount, TransactionType type)
    {
        Id = Guid.NewGuid();
        AccountId = accountId;
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Type = type;
        CreatedAt = DateTime.UtcNow;
    }
}
