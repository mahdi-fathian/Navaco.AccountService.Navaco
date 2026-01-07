namespace Navaco.AccountService.Application.Common;

/// <summary>
/// Handler برای Query
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
