namespace Navaco.AccountService.Application.Queries.GetAccountById;

/// <summary>
/// DTO برای نمایش اطلاعات حساب
/// </summary>
public sealed record AccountDto(
    Guid Id,
    Guid CustomerId,
    decimal Balance,
    string Currency,
    string Status,
    DateTime CreatedAt,
    IReadOnlyCollection<TransactionDto> Transactions);

/// <summary>
/// DTO برای نمایش تراکنش
/// </summary>
public sealed record TransactionDto(
    Guid Id,
    decimal Amount,
    string Currency,
    string Type,
    DateTime CreatedAt);
