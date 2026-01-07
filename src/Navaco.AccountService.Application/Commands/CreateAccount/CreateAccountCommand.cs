namespace Navaco.AccountService.Application.Commands.CreateAccount;

/// <summary>
/// Command برای ایجاد حساب جدید
/// </summary>
public sealed record CreateAccountCommand(
    Guid CustomerId,
    decimal InitialBalance,
    string Currency = "IRR") : ICommand<Guid>;
