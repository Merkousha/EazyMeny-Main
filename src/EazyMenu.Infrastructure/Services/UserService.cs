using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Auth;
using EazyMenu.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EazyMenu.Infrastructure.Services;

/// <summary>
/// پیاده‌سازی سرویس مدیریت کاربران
/// </summary>
public class UserService : IUserService
{
    private readonly UserManager<ApplicationIdentityUser> _userManager;

    public UserService(UserManager<ApplicationIdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AuthResult> ValidateUserCredentialsAsync(
        string phoneOrEmail, 
        string password, 
        CancellationToken cancellationToken = default)
    {
        // جستجوی کاربر
        ApplicationIdentityUser? user = null;

        // اگر ایمیل است
        if (phoneOrEmail.Contains("@"))
        {
            user = await _userManager.FindByEmailAsync(phoneOrEmail);
        }
        else
        {
            // اگر شماره موبایل است
            user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneOrEmail, cancellationToken);
        }

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

        // بررسی رمز عبور
        var passwordValid = await _userManager.CheckPasswordAsync(user, password);

        if (!passwordValid)
        {
            return new AuthResult
            {
                Success = false,
                Message = "شماره موبایل/ایمیل یا رمز عبور اشتباه است"
            };
        }

        // ورود موفق - به‌روزرسانی زمان آخرین ورود
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return new AuthResult
        {
            Success = true,
            Message = "ورود موفقیت‌آمیز",
            User = new UserInfoDto
            {
                Id = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ProfileImageUrl = null, // TODO: اضافه کردن به ApplicationIdentityUser
                PreferredLanguage = "fa",
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public async Task<AuthResult> CreateUserAsync(
        string fullName, 
        string phoneNumber, 
        string email, 
        string password, 
        CancellationToken cancellationToken = default)
    {
        // بررسی وجود کاربر با همین شماره موبایل
        var existingUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);

        if (existingUser != null)
        {
            return new AuthResult
            {
                Success = false,
                Message = "کاربری با این شماره موبایل قبلاً ثبت‌نام کرده است"
            };
        }

        // بررسی وجود کاربر با همین ایمیل
        var existingEmail = await _userManager.FindByEmailAsync(email);
        if (existingEmail != null)
        {
            return new AuthResult
            {
                Success = false,
                Message = "کاربری با این ایمیل قبلاً ثبت‌نام کرده است"
            };
        }

        // ایجاد کاربر جدید
        var user = new ApplicationIdentityUser
        {
            UserName = phoneNumber, // استفاده از شماره موبایل به عنوان UserName
            PhoneNumber = phoneNumber,
            Email = email,
            FullName = fullName,
            PhoneNumberConfirmed = true, // در حالت Production باید OTP تایید شود
            EmailConfirmed = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new AuthResult
            {
                Success = false,
                Message = $"خطا در ثبت‌نام: {errors}"
            };
        }

        // افزودن نقش پیش‌فرض (Customer یا RestaurantOwner)
        await _userManager.AddToRoleAsync(user, "Customer");

        return new AuthResult
        {
            Success = true,
            Message = "ثبت‌نام با موفقیت انجام شد",
            User = new UserInfoDto
            {
                Id = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                ProfileImageUrl = null,
                PreferredLanguage = "fa",
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public async Task<UserInfoDto?> GetUserByPhoneOrEmailAsync(
        string phoneOrEmail, 
        CancellationToken cancellationToken = default)
    {
        ApplicationIdentityUser? user = null;

        if (phoneOrEmail.Contains("@"))
        {
            user = await _userManager.FindByEmailAsync(phoneOrEmail);
        }
        else
        {
            user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneOrEmail, cancellationToken);
        }

        if (user == null) return null;

        return new UserInfoDto
        {
            Id = user.Id,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Email = user.Email ?? string.Empty,
            ProfileImageUrl = null,
            PreferredLanguage = "fa",
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
