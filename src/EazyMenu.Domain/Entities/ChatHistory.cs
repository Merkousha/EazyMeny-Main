using EazyMenu.Domain.Common;

namespace EazyMenu.Domain.Entities;

/// <summary>
/// سابقه گفت‌وگو با هوش مصنوعی
/// </summary>
public class ChatHistory : BaseEntity
{
    /// <summary>
    /// شناسه رستوران
    /// </summary>
    public Guid RestaurantId { get; set; }

    /// <summary>
    /// رستوران مرتبط
    /// </summary>
    public virtual Restaurant Restaurant { get; set; } = null!;

    /// <summary>
    /// شناسه نشست (Session ID)
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// پیام کاربر
    /// </summary>
    public string UserMessage { get; set; } = string.Empty;

    /// <summary>
    /// پاسخ هوش مصنوعی
    /// </summary>
    public string AiResponse { get; set; } = string.Empty;

    /// <summary>
    /// زمان ارسال پیام
    /// </summary>
    public DateTime MessageTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// آیا پیام توسط کاربر بوده؟
    /// </summary>
    public bool IsUserMessage { get; set; } = true;
}
