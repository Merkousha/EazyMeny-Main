using EazyMenu.Application.Common.Interfaces;
using MediatR;

namespace EazyMenu.Application.Features.Auth.Commands.SendOtp;

/// <summary>
/// Handler برای ارسال کد OTP
/// </summary>
public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, SendOtpResult>
{
    private readonly ISmsService _smsService;
    private readonly IOtpService _otpService;
    private const int OtpExpirationMinutes = 2;

    public SendOtpCommandHandler(
        ISmsService smsService,
        IOtpService otpService)
    {
        _smsService = smsService;
        _otpService = otpService;
    }

    public async Task<SendOtpResult> Handle(SendOtpCommand request, CancellationToken cancellationToken)
    {
        // تولید و ذخیره کد OTP
        var otpCode = await _otpService.GenerateOtpAsync(request.PhoneNumber, cancellationToken);

        // ارسال پیامک
        try
        {
            await _smsService.SendAsync(
                request.PhoneNumber,
                $"کد تایید شما: {otpCode}\n\nایزی‌منو",
                cancellationToken);

            return new SendOtpResult
            {
                Success = true,
                Message = $"کد تایید به شماره {request.PhoneNumber} ارسال شد",
                ExpiresInSeconds = OtpExpirationMinutes * 60
            };
        }
        catch (Exception ex)
        {
            // حذف از Cache در صورت خطا
            await _otpService.RemoveOtpAsync(request.PhoneNumber, cancellationToken);

            return new SendOtpResult
            {
                Success = false,
                Message = $"خطا در ارسال پیامک: {ex.Message}"
            };
        }
    }
}
