using System.ComponentModel.DataAnnotations;

namespace Navaco.AccountService.Api.Dtos.Requests;

/// <summary>
/// درخواست واریز به حساب
/// </summary>
/// <param name="Amount">مبلغ واریز (باید بزرگتر از صفر باشد)</param>
/// <param name="Currency">واحد پول (پیش‌فرض: IRR)</param>
public sealed record DepositRequest(
    [Required(ErrorMessage = "مبلغ واریز الزامی است")]
    [Range(0.01, double.MaxValue, ErrorMessage = "مبلغ واریز باید بزرگتر از صفر باشد")]
    decimal Amount,
    
    [StringLength(3, MinimumLength = 3, ErrorMessage = "واحد پول باید ۳ کاراکتر باشد")]
    string Currency = "IRR");
