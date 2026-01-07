namespace Navaco.AccountService.Domain.Events;

/// <summary>
/// رویداد واریز به حساب
/// </summary>
public sealed record DepositMadeEvent(
    Guid AccountId,
    Guid TransactionId,
    decimal Amount,
    string Currency,
    decimal NewBalance) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
