namespace Navaco.AccountService.Application.Common;

/// <summary>
/// Command بدون خروجی
/// </summary>
public interface ICommand : IRequest<Result>
{
}

/// <summary>
/// Command با خروجی
/// </summary>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
