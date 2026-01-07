namespace Navaco.AccountService.Application.Commands.Deposit;

/// <summary>
/// Handler برای واریز به حساب
/// </summary>
public sealed class DepositHandler : ICommandHandler<DepositCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DepositHandler> _logger;

    public DepositHandler(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        ILogger<DepositHandler> logger)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

        if (account is null)
        {
            _logger.LogWarning("حساب با شناسه {AccountId} یافت نشد", request.AccountId);
            return Result.Failure(Error.NotFound("Account", request.AccountId));
        }

        try
        {
            var amount = new Money(request.Amount, request.Currency);
            account.Deposit(amount);

            await _accountRepository.UpdateAsync(account, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "مبلغ {Amount} {Currency} به حساب {AccountId} واریز شد",
                request.Amount,
                request.Currency,
                request.AccountId);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "خطای دامنه در واریز به حساب {AccountId}", request.AccountId);
            return Result.Failure(Error.Domain(ex.Message));
        }
    }
}
