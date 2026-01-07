namespace Navaco.AccountService.Domain.Events;

/// <summary>
/// رویداد ایجاد حساب جدید
/// </summary>
public sealed record AccountCreatedEvent(
    Guid AccountId,
    Guid CustomerId,
    decimal InitialBalance,
    string Currency) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
