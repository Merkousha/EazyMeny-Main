namespace EazyMenu.Application.Common.Models.Auth;

/// <summary>
/// DTO برای ثبت‌نام کاربر جدید
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// نام کامل
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// شماره موبایل (11 رقمی، شروع با 09)
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// ایمیل (اختیاری)
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// رمز عبور (حداقل 6 کاراکتر)
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// تکرار رمز عبور
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;
    
    /// <summary>
    /// قبول قوانین و مقررات
    /// </summary>
    public bool AcceptTerms { get; set; }
}
