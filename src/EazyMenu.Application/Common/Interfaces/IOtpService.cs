namespace EazyMenu.Application.Common.Interfaces;

/// <summary>
/// سرویس مدیریت OTP (One-Time Password)
/// </summary>
public interface IOtpService
{
    /// <summary>
    /// تولید و ذخیره کد OTP
    /// </summary>
    Task<string> GenerateOtpAsync(string phoneNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// بررسی صحت کد OTP
    /// </summary>
    Task<bool> VerifyOtpAsync(string phoneNumber, string code, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// حذف کد OTP (بعد از استفاده)
    /// </summary>
    Task RemoveOtpAsync(string phoneNumber, CancellationToken cancellationToken = default);
}
