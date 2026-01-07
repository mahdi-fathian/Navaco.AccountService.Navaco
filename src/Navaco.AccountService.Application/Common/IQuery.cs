namespace Navaco.AccountService.Application.Common;

/// <summary>
/// Query برای دریافت داده - فقط خواندنی
/// </summary>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
