namespace EazyMenu.Application.Common.Models.Auth;

/// <summary>
/// DTO برای درخواست ارسال کد OTP
/// </summary>
public class OtpRequestDto
{
    /// <summary>
    /// شماره موبایل (11 رقمی، شروع با 09)
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;
}
