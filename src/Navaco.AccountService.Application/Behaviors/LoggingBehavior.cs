using System.Diagnostics;

namespace Navaco.AccountService.Application.Behaviors;

/// <summary>
/// Pipeline Behavior برای لاگ‌گیری خودکار
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "شروع پردازش {RequestName} در {Timestamp}",
            requestName,
            DateTime.UtcNow);

        try
        {
            var response = await next();

            stopwatch.Stop();

            _logger.LogInformation(
                "پایان پردازش {RequestName} در {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "خطا در پردازش {RequestName} پس از {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
