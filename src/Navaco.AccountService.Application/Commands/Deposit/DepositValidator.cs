namespace Navaco.AccountService.Application.Commands.Deposit;

/// <summary>
/// اعتبارسنجی Command واریز
/// </summary>
public sealed class DepositValidator : AbstractValidator<DepositCommand>
{
    public DepositValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("شناسه حساب الزامی است.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("مبلغ واریز باید بیشتر از صفر باشد.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("واحد پول الزامی است.");
    }
}
