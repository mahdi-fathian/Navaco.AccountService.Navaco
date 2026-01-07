namespace Navaco.AccountService.Application.Commands.CreateAccount;

/// <summary>
/// Handler برای ایجاد حساب جدید
/// </summary>
public sealed class CreateAccountHandler : ICommandHandler<CreateAccountCommand, Guid>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAccountHandler> _logger;

    public CreateAccountHandler(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateAccountHandler> logger)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        CreateAccountCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var initialBalance = new Money(request.InitialBalance, request.Currency);
            var account = new Account(request.CustomerId, initialBalance);

            await _accountRepository.AddAsync(account, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "حساب جدید با شناسه {AccountId} برای مشتری {CustomerId} ایجاد شد",
                account.Id,
                request.CustomerId);

            return Result.Success(account.Id);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "خطای دامنه در ایجاد حساب");
            return Result.Failure<Guid>(Error.Domain(ex.Message));
        }
    }
}
