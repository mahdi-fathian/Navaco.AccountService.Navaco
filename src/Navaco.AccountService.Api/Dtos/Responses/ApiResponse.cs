namespace Navaco.AccountService.Api.Dtos.Responses;

/// <summary>
/// پاسخ استاندارد API با داده
/// </summary>
/// <typeparam name="T">نوع داده خروجی</typeparam>
/// <param name="IsSuccess">آیا عملیات موفق بوده است</param>
/// <param name="Data">داده خروجی (در صورت موفقیت)</param>
/// <param name="ErrorCode">کد خطا (در صورت شکست)</param>
/// <param name="ErrorMessage">پیام خطا (در صورت شکست)</param>
/// <param name="TraceId">شناسه ردیابی درخواست</param>
/// <param name="Timestamp">زمان پاسخ</param>
public sealed record ApiResponse<T>(
    bool IsSuccess,
    T? Data,
    string? ErrorCode,
    string? ErrorMessage,
    string TraceId,
    DateTime Timestamp)
{
    /// <summary>
    /// ایجاد پاسخ موفق
    /// </summary>
    public static ApiResponse<T> Success(T data, string traceId) =>
        new(true, data, null, null, traceId, DateTime.UtcNow);

    /// <summary>
    /// ایجاد پاسخ ناموفق
    /// </summary>
    public static ApiResponse<T> Failure(string errorCode, string errorMessage, string traceId) =>
        new(false, default, errorCode, errorMessage, traceId, DateTime.UtcNow);
}

/// <summary>
/// پاسخ استاندارد API بدون داده
/// </summary>
/// <param name="IsSuccess">آیا عملیات موفق بوده است</param>
/// <param name="ErrorCode">کد خطا (در صورت شکست)</param>
/// <param name="ErrorMessage">پیام خطا (در صورت شکست)</param>
/// <param name="TraceId">شناسه ردیابی درخواست</param>
/// <param name="Timestamp">زمان پاسخ</param>
public sealed record ApiResponse(
    bool IsSuccess,
    string? ErrorCode,
    string? ErrorMessage,
    string TraceId,
    DateTime Timestamp)
{
    /// <summary>
    /// ایجاد پاسخ موفق
    /// </summary>
    public static ApiResponse Success(string traceId) =>
        new(true, null, null, traceId, DateTime.UtcNow);

    /// <summary>
    /// ایجاد پاسخ ناموفق
    /// </summary>
    public static ApiResponse Failure(string errorCode, string errorMessage, string traceId) =>
        new(false, errorCode, errorMessage, traceId, DateTime.UtcNow);
}
