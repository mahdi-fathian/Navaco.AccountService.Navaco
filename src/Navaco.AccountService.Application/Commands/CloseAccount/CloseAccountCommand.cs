namespace Navaco.AccountService.Application.Commands.CloseAccount;

/// <summary>
/// Command برای بستن حساب
/// </summary>
public sealed record CloseAccountCommand(Guid AccountId) : ICommand;
