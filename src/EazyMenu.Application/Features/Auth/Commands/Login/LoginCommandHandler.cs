using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Auth;
using MediatR;

namespace EazyMenu.Application.Features.Auth.Commands.Login;

/// <summary>
/// Handler برای ورود با رمز عبور
/// ⚠️ نکته: این Handler فقط اعتبارسنجی می‌کند
/// Controller باید UserManager را برای SignIn استفاده کند
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly IUserService _userService;

    public LoginCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // استفاده از UserService برای اعتبارسنجی
        return await _userService.ValidateUserCredentialsAsync(
            request.PhoneOrEmail, 
            request.Password, 
            cancellationToken);
    }
}
