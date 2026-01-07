namespace Navaco.AccountService.Application.Commands.Withdraw;

/// <summary>
/// Command برای برداشت از حساب
/// </summary>
public sealed record WithdrawCommand(
    Guid AccountId,
    decimal Amount,
    string Currency = "IRR") : ICommand;
