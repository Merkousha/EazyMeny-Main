namespace EazyMenu.Application.Common.Interfaces;

/// <summary>
/// سرویس Hash کردن رمز عبور
/// </summary>
public interface IPasswordHasherService
{
    /// <summary>
    /// Hash کردن رمز عبور
    /// </summary>
    string HashPassword(string password);
    
    /// <summary>
    /// بررسی صحت رمز عبور
    /// </summary>
    bool VerifyPassword(string hashedPassword, string providedPassword);
}
