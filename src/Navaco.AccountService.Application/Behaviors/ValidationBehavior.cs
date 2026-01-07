namespace Navaco.AccountService.Application.Behaviors;

/// <summary>
/// Pipeline Behavior برای اعتبارسنجی خودکار Command/Query ها
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
        {
            _logger.LogWarning(
                "اعتبارسنجی برای {RequestType} با خطا مواجه شد: {Errors}",
                typeof(TRequest).Name,
                string.Join(", ", failures.Select(f => f.ErrorMessage)));

            throw new Application.Exceptions.ValidationException(failures);
        }

        return await next();
    }
}
