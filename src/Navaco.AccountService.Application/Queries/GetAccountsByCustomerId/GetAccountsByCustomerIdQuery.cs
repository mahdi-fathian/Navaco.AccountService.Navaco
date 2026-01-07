namespace Navaco.AccountService.Application.Queries.GetAccountsByCustomerId;

/// <summary>
/// Query برای دریافت حساب‌های یک مشتری
/// </summary>
public sealed record GetAccountsByCustomerIdQuery(Guid CustomerId) : IQuery<IReadOnlyCollection<AccountSummaryDto>>;

/// <summary>
/// DTO خلاصه حساب
/// </summary>
public sealed record AccountSummaryDto(
    Guid Id,
    decimal Balance,
    string Currency,
    string Status,
    DateTime CreatedAt);
