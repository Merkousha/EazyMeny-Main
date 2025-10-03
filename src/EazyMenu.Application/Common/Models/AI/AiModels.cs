namespace EazyMenu.Application.Common.Models.AI;

/// <summary>
/// DTO تنظیمات هوش مصنوعی
/// </summary>
public class AiSettingsDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; }
    public bool IsActive { get; set; }
    public string Environment { get; set; } = string.Empty;
}

/// <summary>
/// DTO درخواست تولید محتوا
/// </summary>
public class GenerateContentRequest
{
    public string ProductName { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty;
    public string Tone { get; set; } = "صمیمی";
}

/// <summary>
/// DTO درخواست تولید تصویر
/// </summary>
public class GenerateImageRequest
{
    public string Description { get; set; } = string.Empty;
    public string Style { get; set; } = "واقعی";
    public int Width { get; set; } = 512;
    public int Height { get; set; } = 512;
}

/// <summary>
/// DTO پیام چت
/// </summary>
public class ChatMessageDto
{
    public string SessionId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsUserMessage { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// DTO پاسخ چت
/// </summary>
public class ChatResponseDto
{
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}
