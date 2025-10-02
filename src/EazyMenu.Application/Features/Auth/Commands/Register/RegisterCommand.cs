using EazyMenu.Application.Common.Models.Auth;
using MediatR;

namespace EazyMenu.Application.Features.Auth.Commands.Register;

/// <summary>
/// Command برای ثبت‌نام کاربر جدید
/// </summary>
public class RegisterCommand : IRequest<AuthResult>
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public bool AcceptTerms { get; set; }
}
