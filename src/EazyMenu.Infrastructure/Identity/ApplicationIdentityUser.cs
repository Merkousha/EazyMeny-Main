using Microsoft.AspNetCore.Identity;

namespace EazyMenu.Infrastructure.Identity;

/// <summary>
/// کلاس Identity User برای Infrastructure
/// </summary>
public class ApplicationIdentityUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
}
