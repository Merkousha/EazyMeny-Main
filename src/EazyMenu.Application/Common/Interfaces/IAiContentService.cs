namespace EazyMenu.Application.Common.Interfaces;

/// <summary>
/// سرویس محتوای هوش مصنوعی
/// </summary>
public interface IAiContentService
{
    /// <summary>
    /// تولید توضیحات محصول با هوش مصنوعی
    /// </summary>
    /// <param name="restaurantId">شناسه رستوران</param>
    /// <param name="productName">نام محصول</param>
    /// <param name="ingredients">مواد تشکیل‌دهنده</param>
    /// <param name="tone">لحن (رسمی، صمیمی، خلاقانه)</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>محتوای تولید شده</returns>
    Task<AiGeneratedContentResult> GenerateProductDescriptionAsync(
        Guid restaurantId,
        string productName,
        string ingredients,
        string tone = "صمیمی",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// تولید تصویر محصول با هوش مصنوعی
    /// </summary>
    /// <param name="restaurantId">شناسه رستوران</param>
    /// <param name="description">توضیحات محصول</param>
    /// <param name="style">سبک تصویری (واقعی، مینیمال، هنری)</param>
    /// <param name="width">عرض تصویر</param>
    /// <param name="height">ارتفاع تصویر</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>داده‌های باینری تصویر</returns>
    Task<byte[]> GenerateProductImageAsync(
        Guid restaurantId,
        string productName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// پاسخ به پیام کاربر در چت
    /// </summary>
    /// <param name="restaurantId">شناسه رستوران</param>
    /// <param name="userMessage">پیام کاربر</param>
    /// <param name="menuContext">اطلاعات منو</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>پاسخ هوش مصنوعی</returns>
    Task<string> GenerateChatResponseAsync(
        Guid restaurantId,
        string userMessage,
        string menuContext,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// نتیجه تولید محتوا با هوش مصنوعی
/// </summary>
public class AiGeneratedContentResult
{
    /// <summary>
    /// عنوان پیشنهادی
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// توضیح کوتاه
    /// </summary>
    public string ShortDescription { get; set; } = string.Empty;

    /// <summary>
    /// توضیح کامل
    /// </summary>
    public string LongDescription { get; set; } = string.Empty;

    /// <summary>
    /// کلیدواژه‌های پیشنهادی
    /// </summary>
    public List<string> Keywords { get; set; } = new();

    /// <summary>
    /// آیا موفق بوده؟
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// پیام خطا (در صورت وجود)
    /// </summary>
    public string? ErrorMessage { get; set; }
}
