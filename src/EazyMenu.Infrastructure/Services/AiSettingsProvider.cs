using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EazyMenu.Infrastructure.Services;

/// <summary>
/// سرویس ارائه‌دهنده تنظیمات هوش مصنوعی
/// </summary>
public class AiSettingsProvider : IAiSettingsProvider
{
    private readonly IRepository<AiSettings> _repository;
    private readonly ILogger<AiSettingsProvider> _logger;

    public AiSettingsProvider(
        IRepository<AiSettings> repository,
        ILogger<AiSettingsProvider> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<string> GetBaseUrlAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        var settings = await GetSettingsAsync(restaurantId, cancellationToken);
        return settings?.BaseUrl ?? string.Empty;
    }

    public async Task<string> GetApiKeyAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        var settings = await GetSettingsAsync(restaurantId, cancellationToken);
        return settings?.ApiKey ?? string.Empty;
    }

    public async Task<string> GetModelNameAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        var settings = await GetSettingsAsync(restaurantId, cancellationToken);
        return settings?.ModelName ?? string.Empty;
    }

    public async Task<int> GetTimeoutAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        var settings = await GetSettingsAsync(restaurantId, cancellationToken);
        return settings?.TimeoutSeconds ?? 30;
    }

    public async Task<bool> HasSettingsAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        var settings = await GetSettingsAsync(restaurantId, cancellationToken);
        return settings != null && settings.IsActive && !string.IsNullOrEmpty(settings.BaseUrl);
    }

    public async Task SaveSettingsAsync(
        Guid restaurantId,
        string baseUrl,
        string apiKey,
        string modelName,
        int timeout,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var existingSettings = await GetSettingsAsync(restaurantId, cancellationToken);

            if (existingSettings != null)
            {
                existingSettings.BaseUrl = baseUrl;
                existingSettings.ApiKey = apiKey;
                existingSettings.ModelName = modelName;
                existingSettings.TimeoutSeconds = timeout;
                existingSettings.IsActive = true;

                await _repository.UpdateAsync(existingSettings, cancellationToken);
            }
            else
            {
                var newSettings = new AiSettings
                {
                    RestaurantId = restaurantId,
                    BaseUrl = baseUrl,
                    ApiKey = apiKey,
                    ModelName = modelName,
                    TimeoutSeconds = timeout,
                    IsActive = true,
                    Environment = "Production"
                };

                await _repository.AddAsync(newSettings, cancellationToken);
            }

            _logger.LogInformation("تنظیمات AI برای رستوران {RestaurantId} ذخیره شد", restaurantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ذخیره تنظیمات AI برای رستوران {RestaurantId}", restaurantId);
            throw;
        }
    }

    public async Task<bool> TestConnectionAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var settings = await GetSettingsAsync(restaurantId, cancellationToken);

            if (settings == null || !settings.IsActive)
            {
                _logger.LogWarning("تنظیمات AI برای رستوران {RestaurantId} یافت نشد یا غیرفعال است", restaurantId);
                return false;
            }

            // تست ساده اتصال با ارسال یک درخواست ساده
            using var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds)
            };

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.ApiKey}");

            // بررسی دسترسی به BaseUrl
            var response = await httpClient.GetAsync(settings.BaseUrl, cancellationToken);

            _logger.LogInformation("تست اتصال AI برای رستوران {RestaurantId}: {StatusCode}", 
                restaurantId, response.StatusCode);

            return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Unauthorized;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در تست اتصال AI برای رستوران {RestaurantId}", restaurantId);
            return false;
        }
    }

    private async Task<AiSettings?> GetSettingsAsync(Guid restaurantId, CancellationToken cancellationToken)
    {
        var results = await _repository.FindAsync(
            x => x.RestaurantId == restaurantId && !x.IsDeleted && x.IsActive,
            cancellationToken);
        
        return results.FirstOrDefault();
    }
}
