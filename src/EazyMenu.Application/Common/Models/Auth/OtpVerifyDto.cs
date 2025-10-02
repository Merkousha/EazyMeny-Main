namespace EazyMenu.Application.Common.Models.Auth;

/// <summary>
/// DTO برای تایید کد OTP
/// </summary>
public class OtpVerifyDto
{
    /// <summary>
    /// شماره موبایل
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// کد OTP (4-6 رقم)
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// مرا به خاطر بسپار
    /// </summary>
    public bool RememberMe { get; set; }
}
