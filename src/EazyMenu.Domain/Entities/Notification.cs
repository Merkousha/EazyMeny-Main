using EazyMenu.Domain.Common;

namespace EazyMenu.Domain.Entities;

/// <summary>
/// Entity اعلان (پیامک و Notification)
/// </summary>
public class Notification : BaseEntity
{
    public Guid? UserId { get; set; }
    public virtual ApplicationUser? User { get; set; }
    
    public string RecipientPhone { get; set; } = string.Empty;
    public string? RecipientEmail { get; set; }
    
    // نوع
    public NotificationType Type { get; set; }
    
    // محتوا
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    
    // وضعیت
    public bool IsSent { get; set; } = false;
    public DateTime? SentAt { get; set; }
    
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    
    // خطا
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; } = 0;
}

public enum NotificationType
{
    SMS = 1,
    Email = 2,
    InApp = 3,
    Push = 4
}
