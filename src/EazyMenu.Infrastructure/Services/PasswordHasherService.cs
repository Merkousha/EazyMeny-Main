using EazyMenu.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace EazyMenu.Infrastructure.Services;

/// <summary>
/// پیاده‌سازی سرویس Hash کردن رمز عبور با ASP.NET Core Identity
/// </summary>
public class PasswordHasherService : IPasswordHasherService
{
    private readonly IPasswordHasher<object> _passwordHasher;

    public PasswordHasherService()
    {
        _passwordHasher = new PasswordHasher<object>();
    }

    /// <summary>
    /// Hash کردن رمز عبور
    /// </summary>
    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(null!, password);
    }

    /// <summary>
    /// بررسی صحت رمز عبور
    /// </summary>
    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(null!, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success || 
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
