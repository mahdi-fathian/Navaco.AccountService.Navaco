using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using Navaco.AccountService.Api.Dtos.Requests;
using Navaco.AccountService.Api.Dtos.Responses;
using Navaco.AccountService.Application.Commands.CloseAccount;
using Navaco.AccountService.Application.Commands.CreateAccount;
using Navaco.AccountService.Application.Commands.Deposit;
using Navaco.AccountService.Application.Commands.Withdraw;
using Navaco.AccountService.Application.Queries.GetAccountById;
using Navaco.AccountService.Application.Queries.GetAccountsByCustomerId;

namespace Navaco.AccountService.Api.Controllers;

/// <summary>
/// کنترلر مدیریت حساب‌های بانکی
/// </summary>
/// <remarks>
/// این کنترلر شامل عملیات‌های اصلی مدیریت حساب شامل ایجاد، واریز، برداشت و بستن حساب است.
/// </remarks>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[SwaggerTag("عملیات مدیریت حساب‌های بانکی")]
public sealed class AccountsController : ControllerBase
{
    private readonly ISender _sender;

    public AccountsController(ISender sender)
    {
        _sender = sender;
    }

    private string TraceId => Activity.Current?.Id ?? HttpContext.TraceIdentifier;

    /// <summary>
    /// ایجاد حساب جدید
    /// </summary>
    /// <remarks>
    /// یک حساب جدید برای مشتری ایجاد می‌کند.
    /// 
    /// نمونه درخواست:
    /// 
    ///     POST /api/v1/accounts
    ///     {
    ///         "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "initialBalance": 1000000,
    ///         "currency": "IRR"
    ///     }
    /// 
    /// </remarks>
    /// <param name="request">اطلاعات حساب جدید</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>شناسه حساب ایجاد شده</returns>
    /// <response code="201">حساب با موفقیت ایجاد شد</response>
    /// <response code="400">خطای اعتبارسنجی ورودی</response>
    /// <response code="422">خطای قوانین کسب‌وکار</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "ایجاد حساب جدید",
        Description = "یک حساب بانکی جدید برای مشتری ایجاد می‌کند",
        OperationId = "CreateAccount")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateAccount(
        [FromBody, SwaggerRequestBody("اطلاعات حساب جدید", Required = true)] CreateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateAccountCommand(
            request.CustomerId,
            request.InitialBalance,
            request.Currency);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return UnprocessableEntity(
                ApiResponse<Guid>.Failure(result.Error.Code, result.Error.Message, TraceId));
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value },
            ApiResponse<Guid>.Success(result.Value, TraceId));
    }

    /// <summary>
    /// دریافت اطلاعات حساب با شناسه
    /// </summary>
    /// <remarks>
    /// اطلاعات کامل یک حساب شامل تراکنش‌ها را برمی‌گرداند.
    /// </remarks>
    /// <param name="id">شناسه یکتای حساب</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>اطلاعات حساب</returns>
    /// <response code="200">اطلاعات حساب با موفقیت دریافت شد</response>
    /// <response code="404">حساب یافت نشد</response>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "دریافت اطلاعات حساب",
        Description = "اطلاعات کامل یک حساب شامل تراکنش‌ها را برمی‌گرداند",
        OperationId = "GetAccountById")]
    [ProducesResponseType(typeof(ApiResponse<AccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute, SwaggerParameter("شناسه یکتای حساب", Required = true)] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetAccountByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(
                ApiResponse<AccountDto>.Failure(result.Error.Code, result.Error.Message, TraceId));
        }

        return Ok(ApiResponse<AccountDto>.Success(result.Value, TraceId));
    }

    /// <summary>
    /// دریافت حساب‌های یک مشتری
    /// </summary>
    /// <remarks>
    /// لیست تمام حساب‌های یک مشتری را برمی‌گرداند.
    /// </remarks>
    /// <param name="customerId">شناسه یکتای مشتری</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>لیست حساب‌های مشتری</returns>
    /// <response code="200">لیست حساب‌ها با موفقیت دریافت شد</response>
    [HttpGet("customer/{customerId:guid}")]
    [SwaggerOperation(
        Summary = "دریافت حساب‌های مشتری",
        Description = "لیست تمام حساب‌های یک مشتری را برمی‌گرداند",
        OperationId = "GetAccountsByCustomerId")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<AccountSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomerId(
        [FromRoute, SwaggerParameter("شناسه یکتای مشتری", Required = true)] Guid customerId,
        CancellationToken cancellationToken)
    {
        var query = new GetAccountsByCustomerIdQuery(customerId);
        var result = await _sender.Send(query, cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<AccountSummaryDto>>.Success(result.Value, TraceId));
    }

    /// <summary>
    /// واریز به حساب
    /// </summary>
    /// <remarks>
    /// مبلغ مشخص شده را به حساب واریز می‌کند.
    /// 
    /// نمونه درخواست:
    /// 
    ///     POST /api/v1/accounts/{id}/deposit
    ///     {
    ///         "amount": 500000,
    ///         "currency": "IRR"
    ///     }
    /// 
    /// </remarks>
    /// <param name="id">شناسه یکتای حساب</param>
    /// <param name="request">اطلاعات واریز</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>نتیجه عملیات</returns>
    /// <response code="200">واریز با موفقیت انجام شد</response>
    /// <response code="400">خطای اعتبارسنجی ورودی</response>
    /// <response code="404">حساب یافت نشد</response>
    /// <response code="422">خطای قوانین کسب‌وکار (مثلاً حساب غیرفعال)</response>
    [HttpPost("{id:guid}/deposit")]
    [SwaggerOperation(
        Summary = "واریز به حساب",
        Description = "مبلغ مشخص شده را به حساب واریز می‌کند",
        OperationId = "Deposit")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Deposit(
        [FromRoute, SwaggerParameter("شناسه یکتای حساب", Required = true)] Guid id,
        [FromBody, SwaggerRequestBody("اطلاعات واریز", Required = true)] DepositRequest request,
        CancellationToken cancellationToken)
    {
        var command = new DepositCommand(id, request.Amount, request.Currency);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            var statusCode = result.Error.Code.Contains("NotFound")
                ? StatusCodes.Status404NotFound
                : StatusCodes.Status422UnprocessableEntity;

            return StatusCode(statusCode,
                ApiResponse.Failure(result.Error.Code, result.Error.Message, TraceId));
        }

        return Ok(ApiResponse.Success(TraceId));
    }

    /// <summary>
    /// برداشت از حساب
    /// </summary>
    /// <remarks>
    /// مبلغ مشخص شده را از حساب برداشت می‌کند.
    /// 
    /// نمونه درخواست:
    /// 
    ///     POST /api/v1/accounts/{id}/withdraw
    ///     {
    ///         "amount": 200000,
    ///         "currency": "IRR"
    ///     }
    /// 
    /// قوانین:
    /// - مبلغ برداشت باید بزرگتر از صفر باشد
    /// - موجودی حساب باید کافی باشد
    /// - حساب باید فعال باشد
    /// </remarks>
    /// <param name="id">شناسه یکتای حساب</param>
    /// <param name="request">اطلاعات برداشت</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>نتیجه عملیات</returns>
    /// <response code="200">برداشت با موفقیت انجام شد</response>
    /// <response code="400">خطای اعتبارسنجی ورودی</response>
    /// <response code="404">حساب یافت نشد</response>
    /// <response code="422">خطای قوانین کسب‌وکار (موجودی ناکافی یا حساب غیرفعال)</response>
    [HttpPost("{id:guid}/withdraw")]
    [SwaggerOperation(
        Summary = "برداشت از حساب",
        Description = "مبلغ مشخص شده را از حساب برداشت می‌کند",
        OperationId = "Withdraw")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Withdraw(
        [FromRoute, SwaggerParameter("شناسه یکتای حساب", Required = true)] Guid id,
        [FromBody, SwaggerRequestBody("اطلاعات برداشت", Required = true)] WithdrawRequest request,
        CancellationToken cancellationToken)
    {
        var command = new WithdrawCommand(id, request.Amount, request.Currency);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            var statusCode = result.Error.Code.Contains("NotFound")
                ? StatusCodes.Status404NotFound
                : StatusCodes.Status422UnprocessableEntity;

            return StatusCode(statusCode,
                ApiResponse.Failure(result.Error.Code, result.Error.Message, TraceId));
        }

        return Ok(ApiResponse.Success(TraceId));
    }

    /// <summary>
    /// بستن حساب
    /// </summary>
    /// <remarks>
    /// حساب را غیرفعال می‌کند. پس از بستن حساب، امکان واریز و برداشت وجود ندارد.
    /// </remarks>
    /// <param name="id">شناسه یکتای حساب</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>نتیجه عملیات</returns>
    /// <response code="200">حساب با موفقیت بسته شد</response>
    /// <response code="404">حساب یافت نشد</response>
    /// <response code="422">خطای قوانین کسب‌وکار (حساب قبلاً بسته شده)</response>
    [HttpPost("{id:guid}/close")]
    [SwaggerOperation(
        Summary = "بستن حساب",
        Description = "حساب را غیرفعال می‌کند",
        OperationId = "CloseAccount")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CloseAccount(
        [FromRoute, SwaggerParameter("شناسه یکتای حساب", Required = true)] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new CloseAccountCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            var statusCode = result.Error.Code.Contains("NotFound")
                ? StatusCodes.Status404NotFound
                : StatusCodes.Status422UnprocessableEntity;

            return StatusCode(statusCode,
                ApiResponse.Failure(result.Error.Code, result.Error.Message, TraceId));
        }

        return Ok(ApiResponse.Success(TraceId));
    }
}
