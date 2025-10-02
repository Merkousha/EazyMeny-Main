using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Auth;
using MediatR;

namespace EazyMenu.Application.Features.Auth.Commands.Register;

/// <summary>
/// Handler برای ثبت‌نام کاربر جدید
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
{
    private readonly IUserService _userService;
    private readonly ISmsService _smsService;

    public RegisterCommandHandler(
        IUserService userService,
        ISmsService smsService)
    {
        _userService = userService;
        _smsService = smsService;
    }

    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // ثبت‌نام کاربر از طریق UserService
        var result = await _userService.CreateUserAsync(
            request.FullName,
            request.PhoneNumber,
            request.Email ?? string.Empty,
            request.Password,
            cancellationToken);

        if (!result.Success)
        {
            return result;
        }

        // ارسال پیامک خوش‌آمدگویی (اختیاری)
        try
        {
            await _smsService.SendAsync(
                request.PhoneNumber,
                $"سلام {request.FullName} عزیز، به ایزی‌منو خوش آمدید!",
                cancellationToken);
        }
        catch
        {
            // اگر ارسال SMS ناموفق بود، مشکلی نیست
        }

        return result;
    }
}
