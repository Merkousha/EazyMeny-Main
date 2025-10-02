using EazyMenu.Application.Common.Models.Auth;

namespace EazyMenu.Application.Common.Interfaces;

/// <summary>
/// سرویس مدیریت کاربران
/// </summary>
public interface IUserService
{
    /// <summary>
    /// اعتبارسنجی اطلاعات ورود کاربر
    /// </summary>
    Task<AuthResult> ValidateUserCredentialsAsync(
        string phoneOrEmail, 
        string password, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ایجاد کاربر جدید
    /// </summary>
    Task<AuthResult> CreateUserAsync(
        string fullName,
        string phoneNumber,
        string email,
        string password,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// یافتن کاربر با شماره موبایل یا ایمیل
    /// </summary>
    Task<UserInfoDto?> GetUserByPhoneOrEmailAsync(
        string phoneOrEmail, 
        CancellationToken cancellationToken = default);
}
