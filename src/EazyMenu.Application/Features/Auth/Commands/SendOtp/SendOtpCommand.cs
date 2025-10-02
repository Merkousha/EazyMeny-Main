using MediatR;

namespace EazyMenu.Application.Features.Auth.Commands.SendOtp;

/// <summary>
/// Command برای ارسال کد OTP
/// </summary>
public class SendOtpCommand : IRequest<SendOtpResult>
{
    /// <summary>
    /// شماره موبایل
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;
}

/// <summary>
/// نتیجه ارسال OTP
/// </summary>
public class SendOtpResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    
    /// <summary>
    /// زمان انقضای کد (ثانیه)
    /// </summary>
    public int ExpiresInSeconds { get; set; }
}
