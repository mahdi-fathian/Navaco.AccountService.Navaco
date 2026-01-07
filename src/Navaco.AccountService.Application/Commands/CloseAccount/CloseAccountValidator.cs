namespace Navaco.AccountService.Application.Commands.CloseAccount;

/// <summary>
/// اعتبارسنجی Command بستن حساب
/// </summary>
public sealed class CloseAccountValidator : AbstractValidator<CloseAccountCommand>
{
    public CloseAccountValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("شناسه حساب الزامی است.");
    }
}
