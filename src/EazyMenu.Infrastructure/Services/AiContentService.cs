using System.ClientModel;
using System.Text;
using System.Text.Json;
using EazyMenu.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI;
using OpenAI.Images;

namespace EazyMenu.Infrastructure.Services;

/// <summary>
/// سرویس محتوای هوش مصنوعی با استفاده از Semantic Kernel
/// </summary>
public class AiContentService : IAiContentService
{
    private readonly IAiSettingsProvider _settingsProvider;
    private readonly ILogger<AiContentService> _logger;

    public AiContentService(
        IAiSettingsProvider settingsProvider,
        ILogger<AiContentService> logger)
    {
        _settingsProvider = settingsProvider;
        _logger = logger;
    }

    public async Task<AiGeneratedContentResult> GenerateProductDescriptionAsync(
        Guid restaurantId,
        string productName,
        string ingredients,
        string tone = "صمیمی",
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("شروع تولید محتوا برای محصول '{ProductName}' در رستوران {RestaurantId}",
                productName, restaurantId);

            // بررسی وجود تنظیمات
            var hasSettings = await _settingsProvider.HasSettingsAsync(restaurantId, cancellationToken);
            if (!hasSettings)
            {
                return new AiGeneratedContentResult
                {
                    IsSuccess = false,
                    ErrorMessage = "تنظیمات هوش مصنوعی یافت نشد. لطفاً ابتدا تنظیمات را وارد کنید."
                };
            }

            // ساخت Kernel
            var kernel = await BuildKernelAsync(restaurantId, cancellationToken);
            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            // ساخت Prompt
            var prompt = BuildProductDescriptionPrompt(productName, ingredients, tone);

            // ارسال درخواست
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage("شما یک نویسنده حرفه‌ای محتوای رستوران هستید که توضیحات جذاب و خوشمزه برای غذاها می‌نویسید.");
            chatHistory.AddUserMessage(prompt);

            var response = await chatService.GetChatMessageContentAsync(
                chatHistory,
                executionSettings: new OpenAIPromptExecutionSettings
                {
                    Temperature = 0.7,
                    MaxTokens = 1000
                },
                cancellationToken: cancellationToken);

            // پردازش پاسخ
            var result = ParseProductDescriptionResponse(response.Content ?? string.Empty);

            _logger.LogInformation("محتوا با موفقیت تولید شد برای محصول '{ProductName}'", productName);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در تولید محتوا برای محصول '{ProductName}'", productName);

            return new AiGeneratedContentResult
            {
                IsSuccess = false,
                ErrorMessage = $"خطا در تولید محتوا: {ex.Message}"
            };
        }
    }

    public async Task<byte[]> GenerateProductImageAsync(
        Guid restaurantId,
        string productName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("شروع تولید تصویر برای رستوران {RestaurantId}", restaurantId);

            // بررسی وجود تنظیمات
            var hasSettings = await _settingsProvider.HasSettingsAsync(restaurantId, cancellationToken);
            if (!hasSettings)
            {
                throw new InvalidOperationException("تنظیمات هوش مصنوعی یافت نشد.");
            }


            // ساخت prompt برای تولید تصویر
            var imagePrompt = BuildImagePrompt(productName);

            var kernel = await BuildImageKernelAsync(restaurantId, cancellationToken);

            // Generate the image
            GeneratedImage generatedImage = await kernel.GenerateImageAsync(imagePrompt);

            // API به صورت پیش‌فرض ImageBytes برمی‌گرداند (نه ImageUri)
            if (generatedImage.ImageBytes != null)
            {
                var imageData = generatedImage.ImageBytes.ToArray();
                
                _logger.LogInformation("تصویر با موفقیت تولید شد برای رستوران {RestaurantId}. حجم: {Size} بایت", 
                    restaurantId, imageData.Length);
                
                return imageData;
            }
            else
            {
                throw new InvalidOperationException("تصویر تولید شده خالی است.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در تولید تصویر برای رستوران {RestaurantId}", restaurantId);
            throw;
        }
    }

    public async Task<string> GenerateChatResponseAsync(
        Guid restaurantId,
        string userMessage,
        string menuContext,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("شروع تولید پاسخ چت برای رستوران {RestaurantId}", restaurantId);

            // بررسی وجود تنظیمات
            var hasSettings = await _settingsProvider.HasSettingsAsync(restaurantId, cancellationToken);
            if (!hasSettings)
            {
                return "متاسفانه سرویس چت در حال حاضر در دسترس نیست.";
            }

            // ساخت Kernel
            var kernel = await BuildKernelAsync(restaurantId, cancellationToken);
            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            // ساخت Prompt
            var systemPrompt = BuildChatSystemPrompt(menuContext);

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemPrompt);
            chatHistory.AddUserMessage(userMessage);

            // ارسال درخواست
            var response = await chatService.GetChatMessageContentAsync(
                chatHistory,
                executionSettings: new OpenAIPromptExecutionSettings
                {
                    Temperature = 0.6,
                    MaxTokens = 2000
                },
                cancellationToken: cancellationToken);

            _logger.LogInformation("پاسخ چت با موفقیت تولید شد برای رستوران {RestaurantId}", restaurantId);

            return response.Content ?? "متاسفانه نتوانستم پاسخی تولید کنم.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در تولید پاسخ چت برای رستوران {RestaurantId}", restaurantId);
            return "متاسفانه در حال حاضر نمی‌توانم پاسخ دهم. لطفاً بعداً تلاش کنید.";
        }
    }

    #region Private Methods

    private async Task<Kernel> BuildKernelAsync(Guid restaurantId, CancellationToken cancellationToken)
    {
        var baseUrl = await _settingsProvider.GetBaseUrlAsync(restaurantId, cancellationToken);
        var apiKey = await _settingsProvider.GetApiKeyAsync(restaurantId, cancellationToken);
        var modelName = await _settingsProvider.GetModelNameAsync(restaurantId, cancellationToken);

        var builder = Kernel.CreateBuilder();


#pragma warning disable SKEXP0010
        builder.AddOpenAIChatCompletion(
            modelId: modelName,
            endpoint: new Uri(baseUrl),
            apiKey: apiKey);


        return builder.Build();
    }
    private async Task<ImageClient> BuildImageKernelAsync(Guid restaurantId, CancellationToken cancellationToken)
    {
        var baseUrl = await _settingsProvider.GetBaseUrlAsync(restaurantId, cancellationToken);
        var apiKey = await _settingsProvider.GetApiKeyAsync(restaurantId, cancellationToken);
        var modelName = "gpt-image-1";

        ApiKeyCredential key = new ApiKeyCredential(apiKey);
        OpenAIClientOptions options = new OpenAIClientOptions
        {
            Endpoint = new Uri(baseUrl)
        };
        ImageClient client = new(modelName, key, options);


        return client;
    }
    private string BuildProductDescriptionPrompt(string productName, string ingredients, string tone)
    {
        var toneDescription = tone switch
        {
            "رسمی" => "رسمی و حرفه‌ای",
            "صمیمی" => "صمیمی و دوستانه",
            "خلاقانه" => "خلاقانه و هنری",
            _ => "صمیمی و دوستانه"
        };

        return $@"
لطفاً برای محصول زیر محتوای جذاب تولید کن:

**نام محصول:** {productName}
**مواد تشکیل‌دهنده:** {ingredients}
**لحن:** {toneDescription}

لطفاً خروجی را در قالب JSON زیر بده:
{{
    ""title"": ""عنوان جذاب"",
    ""shortDescription"": ""توضیح کوتاه (حداکثر 100 کاراکتر)"",
    ""longDescription"": ""توضیح کامل (حداکثر 300 کاراکتر)"",
    ""keywords"": [""کلیدواژه۱"", ""کلیدواژه۲"", ""کلیدواژه۳""]
}}

توضیحات باید به فارسی باشد و غذا را خوشمزه و وسوسه‌انگیز توصیف کند.
";
    }

    private string BuildImagePrompt(string productName)
    {


        return $"A realistic and cozy café menu illustration in a square format, with a warm and inviting atmosphere. The setting features soft natural lighting, a rustic wooden table surface, and a neutral-toned background that remains consistent across all menu items. In the center of the composition, prominently display a {productName} on a simple ceramic plate, rendered with realistic textures and fine detail. Surrounding elements such as cups, saucers, small green plants, and scattered coffee beans remain subtle in the background to create a cohesive café ambiance, while keeping the focus on the {productName}. The artistic style is realistic with a touch of minimalism, using soft shadows and a balanced composition. Colors stay within a warm palette of browns, creams, and muted earthy tones. The perspective is slightly top-down, always centered on the {productName}. No text or labels should appear anywhere in the image.";
    }

    private string BuildChatSystemPrompt(string menuContext)
    {
        return $@"
شما دستیار هوشمند رستوران هستید. وظیفه شما کمک به مشتریان برای انتخاب غذا و پاسخ به سوالات آنهاست.

**منوی رستوران:**
{menuContext}

**دستورالعمل‌ها:**
1. به سوالات مشتریان در مورد غذاها پاسخ دهید
2. پیشنهادهای مفید بر اساس منو ارائه دهید
3. در مورد مواد تشکیل‌دهنده و قیمت‌ها اطلاعات دهید
4. پاسخ‌ها را کوتاه، مفید و دوستانه نگه دارید
5. اگر غذایی در منو نیست، به مشتری بگویید و گزینه‌های مشابه پیشنهاد دهید

همیشه به فارسی پاسخ دهید.
";
    }

    private AiGeneratedContentResult ParseProductDescriptionResponse(string response)
    {
        try
        {
            // تلاش برای پارس JSON
            var jsonStart = response.IndexOf('{');
            var jsonEnd = response.LastIndexOf('}');

            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonString = response.Substring(jsonStart, jsonEnd - jsonStart + 1);
                var jsonDoc = JsonDocument.Parse(jsonString);
                var root = jsonDoc.RootElement;

                return new AiGeneratedContentResult
                {
                    IsSuccess = true,
                    Title = root.GetProperty("title").GetString() ?? string.Empty,
                    ShortDescription = root.GetProperty("shortDescription").GetString() ?? string.Empty,
                    LongDescription = root.GetProperty("longDescription").GetString() ?? string.Empty,
                    Keywords = root.GetProperty("keywords").EnumerateArray()
                        .Select(k => k.GetString() ?? string.Empty)
                        .Where(k => !string.IsNullOrEmpty(k))
                        .ToList()
                };
            }

            // اگر JSON نبود، کل متن را به عنوان توضیح برمی‌گردانیم
            return new AiGeneratedContentResult
            {
                IsSuccess = true,
                Title = "محتوای تولید شده",
                LongDescription = response,
                ShortDescription = response.Length > 100 ? response.Substring(0, 100) + "..." : response,
                Keywords = new List<string>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در پارس پاسخ AI");

            return new AiGeneratedContentResult
            {
                IsSuccess = false,
                ErrorMessage = "خطا در پردازش پاسخ هوش مصنوعی"
            };
        }
    }

    #endregion
}
