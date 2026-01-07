namespace Navaco.AccountService.Application.Commands.CreateAccount;

/// <summary>
/// اعتبارسنجی Command ایجاد حساب
/// </summary>
public sealed class CreateAccountValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("شناسه مشتری الزامی است.");

        RuleFor(x => x.InitialBalance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("موجودی اولیه نمی‌تواند منفی باشد.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("واحد پول الزامی است.")
            .MaximumLength(3)
            .WithMessage("واحد پول باید حداکثر ۳ کاراکتر باشد.");
    }
}
