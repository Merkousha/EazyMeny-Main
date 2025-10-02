using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Auth;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Auth.Commands.Login;

/// <summary>
/// Handler برای ورود با رمز عبور
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly IRepository<ApplicationUser> _userRepository;
    private readonly IPasswordHasherService _passwordHasher;

    public LoginCommandHandler(
        IRepository<ApplicationUser> userRepository,
        IPasswordHasherService passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // جستجوی کاربر با شماره موبایل یا ایمیل
        var users = await _userRepository.FindAsync(
            u => (u.PhoneNumber == request.PhoneOrEmail || u.Email == request.PhoneOrEmail) 
                 && !u.IsDeleted,
            cancellationToken);

        var user = users.FirstOrDefault();

        if (user == null)
        {
            return new AuthResult
            {
                Success = false,
                Message = "شماره موبایل/ایمیل یا رمز عبور اشتباه است"
            };
        }

        // بررسی فعال بودن کاربر
        if (!user.IsActive)
        {
            return new AuthResult
            {
                Success = false,
                Message = "حساب کاربری شما غیرفعال شده است. لطفاً با پشتیبانی تماس بگیرید"
            };
        }

        // بررسی صحت رمز عبور
        var isPasswordValid = _passwordHasher.VerifyPassword(user.PasswordHash, request.Password);

        if (!isPasswordValid)
        {
            return new AuthResult
            {
                Success = false,
                Message = "شماره موبایل/ایمیل یا رمز عبور اشتباه است"
            };
        }

        // ورود موفق
        return new AuthResult
        {
            Success = true,
            Message = "ورود موفقیت‌آمیز",
            User = new UserInfoDto
            {
                Id = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
                PreferredLanguage = user.PreferredLanguage,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            }
        };
    }
}
