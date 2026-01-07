namespace Navaco.AccountService.Application.Commands.Deposit;

/// <summary>
/// Command برای واریز به حساب
/// </summary>
public sealed record DepositCommand(
    Guid AccountId,
    decimal Amount,
    string Currency = "IRR") : ICommand;
