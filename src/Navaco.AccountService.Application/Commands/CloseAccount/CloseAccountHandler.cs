namespace Navaco.AccountService.Application.Commands.CloseAccount;

/// <summary>
/// Handler برای بستن حساب
/// </summary>
public sealed class CloseAccountHandler : ICommandHandler<CloseAccountCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CloseAccountHandler> _logger;

    public CloseAccountHandler(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        ILogger<CloseAccountHandler> logger)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(CloseAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

        if (account is null)
        {
            _logger.LogWarning("حساب با شناسه {AccountId} یافت نشد", request.AccountId);
            return Result.Failure(Error.NotFound("Account", request.AccountId));
        }

        try
        {
            account.Close();

            await _accountRepository.UpdateAsync(account, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("حساب {AccountId} بسته شد", request.AccountId);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "خطای دامنه در بستن حساب {AccountId}", request.AccountId);
            return Result.Failure(Error.Domain(ex.Message));
        }
    }
}
