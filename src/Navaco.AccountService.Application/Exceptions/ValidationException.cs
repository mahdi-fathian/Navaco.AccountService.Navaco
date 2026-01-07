using FluentValidation.Results;

namespace Navaco.AccountService.Application.Exceptions;

/// <summary>
/// استثنای اعتبارسنجی
/// </summary>
public sealed class ValidationException : Exception
{
    public IReadOnlyCollection<ValidationError> Errors { get; }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("یک یا چند خطای اعتبارسنجی رخ داده است.")
    {
        Errors = failures
            .Select(f => new ValidationError(f.PropertyName, f.ErrorMessage))
            .ToList()
            .AsReadOnly();
    }
}

public sealed record ValidationError(string PropertyName, string ErrorMessage);
