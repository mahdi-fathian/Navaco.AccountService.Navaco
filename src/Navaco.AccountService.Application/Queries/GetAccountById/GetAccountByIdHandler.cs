namespace Navaco.AccountService.Application.Queries.GetAccountById;

/// <summary>
/// Handler برای دریافت اطلاعات حساب
/// </summary>
public sealed class GetAccountByIdHandler : IQueryHandler<GetAccountByIdQuery, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<GetAccountByIdHandler> _logger;

    public GetAccountByIdHandler(
        IAccountRepository accountRepository,
        ILogger<GetAccountByIdHandler> logger)
    {
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public async Task<Result<AccountDto>> Handle(
        GetAccountByIdQuery request,
        CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

        if (account is null)
        {
            _logger.LogWarning("حساب با شناسه {AccountId} یافت نشد", request.AccountId);
            return Result.Failure<AccountDto>(Error.NotFound("Account", request.AccountId));
        }

        var dto = MapToDto(account);

        return Result.Success(dto);
    }

    private static AccountDto MapToDto(Account account)
    {
        var transactions = account.Transactions
            .Select(t => new TransactionDto(
                t.Id,
                t.Amount.Amount,
                t.Amount.Currency,
                t.Type.ToString(),
                t.CreatedAt))
            .ToList()
            .AsReadOnly();

        return new AccountDto(
            account.Id,
            account.CustomerId,
            account.Balance.Amount,
            account.Balance.Currency,
            account.Status.ToString(),
            account.CreatedAt,
            transactions);
    }
}
