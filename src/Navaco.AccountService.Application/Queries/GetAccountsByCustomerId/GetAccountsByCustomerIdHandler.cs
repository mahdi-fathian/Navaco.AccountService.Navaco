namespace Navaco.AccountService.Application.Queries.GetAccountsByCustomerId;

/// <summary>
/// Handler برای دریافت حساب‌های مشتری
/// </summary>
public sealed class GetAccountsByCustomerIdHandler 
    : IQueryHandler<GetAccountsByCustomerIdQuery, IReadOnlyCollection<AccountSummaryDto>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<GetAccountsByCustomerIdHandler> _logger;

    public GetAccountsByCustomerIdHandler(
        IAccountRepository accountRepository,
        ILogger<GetAccountsByCustomerIdHandler> logger)
    {
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<AccountSummaryDto>>> Handle(
        GetAccountsByCustomerIdQuery request,
        CancellationToken cancellationToken)
    {
        var accounts = await _accountRepository.GetByCustomerIdAsync(
            request.CustomerId, 
            cancellationToken);

        var dtos = accounts
            .Select(a => new AccountSummaryDto(
                a.Id,
                a.Balance.Amount,
                a.Balance.Currency,
                a.Status.ToString(),
                a.CreatedAt))
            .ToList()
            .AsReadOnly();

        _logger.LogInformation(
            "تعداد {Count} حساب برای مشتری {CustomerId} یافت شد",
            dtos.Count,
            request.CustomerId);

        return Result.Success<IReadOnlyCollection<AccountSummaryDto>>(dtos);
    }
}
