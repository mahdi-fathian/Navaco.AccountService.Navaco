namespace Navaco.AccountService.Application.Commands.Withdraw;

/// <summary>
/// اعتبارسنجی Command برداشت
/// </summary>
public sealed class WithdrawValidator : AbstractValidator<WithdrawCommand>
{
    public WithdrawValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("شناسه حساب الزامی است.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("مبلغ برداشت باید بیشتر از صفر باشد.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("واحد پول الزامی است.");
    }
}
