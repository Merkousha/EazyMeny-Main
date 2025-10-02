namespace EazyMenu.Application.Common.Models.Auth;

/// <summary>
/// اطلاعات خلاصه کاربر
/// </summary>
public class UserInfoDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string PreferredLanguage { get; set; } = "fa";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
