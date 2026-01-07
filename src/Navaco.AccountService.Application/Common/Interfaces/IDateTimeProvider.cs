namespace Navaco.AccountService.Application.Common.Interfaces;

/// <summary>
/// اینترفیس برای دسترسی به زمان جاری
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
