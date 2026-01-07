using Navaco.AccountService.Application.Common.Interfaces;

namespace Navaco.AccountService.Infrastructure.Services;

/// <summary>
/// پیاده‌سازی IDateTimeProvider
/// </summary>
public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
