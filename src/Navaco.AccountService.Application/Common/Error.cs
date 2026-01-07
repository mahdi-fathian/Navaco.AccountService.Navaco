namespace Navaco.AccountService.Application.Common;

/// <summary>
/// مدل خطا
/// </summary>
public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "مقدار نمی‌تواند خالی باشد.");

    public static Error NotFound(string entityName, Guid id) =>
        new($"{entityName}.NotFound", $"{entityName} با شناسه {id} یافت نشد.");

    public static Error Validation(string message) =>
        new("Validation.Error", message);

    public static Error Domain(string message) =>
        new("Domain.Error", message);
}
