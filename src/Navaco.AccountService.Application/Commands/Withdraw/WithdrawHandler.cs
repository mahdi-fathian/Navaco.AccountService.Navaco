namespace Navaco.AccountService.Application.Commands.Withdraw;

/// <summary>
/// Handler برای برداشت از حساب
/// </summary>
public sealed class WithdrawHandler : ICommandHandler<WithdrawCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WithdrawHandler> _logger;

    public WithdrawHandler(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        ILogger<WithdrawHandler> logger)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(WithdrawCommand request, CancellationToken cancellationToken)
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
            account.Withdraw(amount);

            await _accountRepository.UpdateAsync(account, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "مبلغ {Amount} {Currency} از حساب {AccountId} برداشت شد",
                request.Amount,
                request.Currency,
                request.AccountId);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "خطای دامنه در برداشت از حساب {AccountId}", request.AccountId);
            return Result.Failure(Error.Domain(ex.Message));
        }
    }
}
