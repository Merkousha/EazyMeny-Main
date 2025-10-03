using EazyMenu.Domain.Common;

namespace EazyMenu.Domain.Entities;

/// <summary>
/// تنظیمات هوش مصنوعی برای هر رستوران
/// </summary>
public class AiSettings : BaseEntity, IAggregateRoot
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
    /// آدرس پایه API (Base URL)
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// کلید API
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// نام مدل یا Deployment
    /// </summary>
    public string ModelName { get; set; } = string.Empty;

    /// <summary>
    /// زمان انقضا برای درخواست‌ها (به ثانیه)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// آیا تنظیمات فعال است؟
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// محیط استفاده (Development, Production)
    /// </summary>
    public string Environment { get; set; } = "Production";
}
