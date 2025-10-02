using EazyMenu.Application.Common.Models.Auth;
using MediatR;

namespace EazyMenu.Application.Features.Auth.Commands.VerifyOtp;

/// <summary>
/// Command برای تایید کد OTP و ورود
/// </summary>
public class VerifyOtpCommand : IRequest<AuthResult>
{
    /// <summary>
    /// شماره موبایل
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// کد OTP
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// مرا به خاطر بسپار
    /// </summary>
    public bool RememberMe { get; set; }
}
