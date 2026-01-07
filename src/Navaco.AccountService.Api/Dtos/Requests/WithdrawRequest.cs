using System.ComponentModel.DataAnnotations;

namespace Navaco.AccountService.Api.Dtos.Requests;

/// <summary>
/// درخواست برداشت از حساب
/// </summary>
/// <param name="Amount">مبلغ برداشت (باید بزرگتر از صفر باشد)</param>
/// <param name="Currency">واحد پول (پیش‌فرض: IRR)</param>
public sealed record WithdrawRequest(
    [Required(ErrorMessage = "مبلغ برداشت الزامی است")]
    [Range(0.01, double.MaxValue, ErrorMessage = "مبلغ برداشت باید بزرگتر از صفر باشد")]
    decimal Amount,
    
    [StringLength(3, MinimumLength = 3, ErrorMessage = "واحد پول باید ۳ کاراکتر باشد")]
    string Currency = "IRR");
