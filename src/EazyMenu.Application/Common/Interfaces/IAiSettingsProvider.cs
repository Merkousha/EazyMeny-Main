namespace EazyMenu.Application.Common.Interfaces;

/// <summary>
/// سرویس ارائه‌دهنده تنظیمات هوش مصنوعی
/// </summary>
public interface IAiSettingsProvider
{
    /// <summary>
    /// دریافت آدرس پایه API
    /// </summary>
    Task<string> GetBaseUrlAsync(Guid restaurantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت کلید API
    /// </summary>
    Task<string> GetApiKeyAsync(Guid restaurantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت نام مدل
    /// </summary>
    Task<string> GetModelNameAsync(Guid restaurantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت زمان انقضا
    /// </summary>
    Task<int> GetTimeoutAsync(Guid restaurantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی وجود تنظیمات
    /// </summary>
    Task<bool> HasSettingsAsync(Guid restaurantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ذخیره تنظیمات
    /// </summary>
    Task SaveSettingsAsync(Guid restaurantId, string baseUrl, string apiKey, string modelName, int timeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// تست اتصال
    /// </summary>
    Task<bool> TestConnectionAsync(Guid restaurantId, CancellationToken cancellationToken = default);
}
