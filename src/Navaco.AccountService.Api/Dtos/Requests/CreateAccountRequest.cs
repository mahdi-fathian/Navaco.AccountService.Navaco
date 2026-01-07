using System.ComponentModel.DataAnnotations;

namespace Navaco.AccountService.Api.Dtos.Requests;

/// <summary>
/// درخواست ایجاد حساب جدید
/// </summary>
/// <param name="CustomerId">شناسه یکتای مشتری</param>
/// <param name="InitialBalance">موجودی اولیه حساب (باید بزرگتر یا مساوی صفر باشد)</param>
/// <param name="Currency">واحد پول (پیش‌فرض: IRR)</param>
public sealed record CreateAccountRequest(
    [Required(ErrorMessage = "شناسه مشتری الزامی است")]
    Guid CustomerId,
    
    [Range(0, double.MaxValue, ErrorMessage = "موجودی اولیه نمی‌تواند منفی باشد")]
    decimal InitialBalance,
    
    [StringLength(3, MinimumLength = 3, ErrorMessage = "واحد پول باید ۳ کاراکتر باشد")]
    string Currency = "IRR");
