namespace Navaco.AccountService.Application.Queries.GetAccountById;

/// <summary>
/// Query برای دریافت اطلاعات حساب با شناسه
/// </summary>
public sealed record GetAccountByIdQuery(Guid AccountId) : IQuery<AccountDto>;
