using System.Diagnostics;
using System.Text.Json;
using Navaco.AccountService.Api.Dtos.Responses;
using Navaco.AccountService.Application.Exceptions;
using Navaco.AccountService.Domain.Exceptions;

namespace Navaco.AccountService.Api.Middlewares;

/// <summary>
/// Middleware مدیریت استثناء
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var (statusCode, errorCode, message) = exception switch
        {
            ValidationException validationException =>
                (StatusCodes.Status400BadRequest,
                 "Validation.Error",
                 string.Join("; ", validationException.Errors.Select(e => e.ErrorMessage))),

            DomainException domainException =>
                (StatusCodes.Status422UnprocessableEntity,
                 "Domain.Error",
                 domainException.Message),

            _ =>
                (StatusCodes.Status500InternalServerError,
                 "Internal.Error",
                 "خطای داخلی سرور رخ داده است.")
        };

        _logger.LogError(
            exception,
            "خطا در پردازش درخواست. TraceId: {TraceId}, StatusCode: {StatusCode}, ErrorCode: {ErrorCode}",
            traceId,
            statusCode,
            errorCode);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = ApiResponse.Failure(errorCode, message, traceId);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
