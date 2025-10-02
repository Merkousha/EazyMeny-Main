namespace EazyMenu.Application.Common.Models.Auth;

/// <summary>
/// نتیجه عملیات Authentication
/// </summary>
public class AuthResult
{
    /// <summary>
    /// موفقیت‌آمیز بودن عملیات
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// پیام (در صورت خطا)
    /// </summary>
    public string? Message { get; set; }
    
    /// <summary>
    /// JWT Token (در صورت موفقیت)
    /// </summary>
    public string? Token { get; set; }
    
    /// <summary>
    /// Refresh Token (برای تمدید)
    /// </summary>
    public string? RefreshToken { get; set; }
    
    /// <summary>
    /// تاریخ انقضای Token
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
    
    /// <summary>
    /// اطلاعات کاربر
    /// </summary>
    public UserInfoDto? User { get; set; }
}
