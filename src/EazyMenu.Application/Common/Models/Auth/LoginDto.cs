namespace EazyMenu.Application.Common.Models.Auth;

/// <summary>
/// DTO برای ورود با رمز عبور
/// </summary>
public class LoginDto
{
    /// <summary>
    /// شماره موبایل یا ایمیل
    /// </summary>
    public string PhoneOrEmail { get; set; } = string.Empty;
    
    /// <summary>
    /// رمز عبور
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// مرا به خاطر بسپار (30 روز)
    /// </summary>
    public bool RememberMe { get; set; }
}
