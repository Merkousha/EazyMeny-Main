using EazyMenu.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace EazyMenu.Application.Features.AI.Commands.TestAiConnection;

/// <summary>
/// هندلر تست اتصال با سرویس هوش مصنوعی
/// </summary>
public class TestAiConnectionCommandHandler : IRequestHandler<TestAiConnectionCommand, TestConnectionResult>
{
    private readonly IAiSettingsProvider _settingsProvider;
    private readonly ILogger<TestAiConnectionCommandHandler> _logger;

    public TestAiConnectionCommandHandler(
        IAiSettingsProvider settingsProvider,
        ILogger<TestAiConnectionCommandHandler> logger)
    {
        _settingsProvider = settingsProvider;
        _logger = logger;
    }

    public async Task<TestConnectionResult> Handle(TestAiConnectionCommand request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("شروع تست اتصال هوش مصنوعی برای رستوران {RestaurantId}", request.RestaurantId);

            // بررسی وجود تنظیمات
            var hasSettings = await _settingsProvider.HasSettingsAsync(request.RestaurantId, cancellationToken);
            
            if (!hasSettings)
            {
                return new TestConnectionResult
                {
                    IsSuccess = false,
                    Message = "تنظیمات هوش مصنوعی یافت نشد. لطفاً ابتدا تنظیمات را وارد کنید.",
                    ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds
                };
            }

            // تست اتصال
            var isConnected = await _settingsProvider.TestConnectionAsync(request.RestaurantId, cancellationToken);
            
            stopwatch.Stop();

            if (isConnected)
            {
                _logger.LogInformation("اتصال موفق به سرویس هوش مصنوعی برای رستوران {RestaurantId}", request.RestaurantId);
                
                return new TestConnectionResult
                {
                    IsSuccess = true,
                    Message = $"اتصال موفقیت‌آمیز بود. زمان پاسخ: {stopwatch.ElapsedMilliseconds} میلی‌ثانیه",
                    ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds
                };
            }
            else
            {
                _logger.LogWarning("اتصال ناموفق به سرویس هوش مصنوعی برای رستوران {RestaurantId}", request.RestaurantId);
                
                return new TestConnectionResult
                {
                    IsSuccess = false,
                    Message = "اتصال ناموفق. لطفاً تنظیمات را بررسی کنید.",
                    ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds
                };
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "خطا در تست اتصال هوش مصنوعی برای رستوران {RestaurantId}", request.RestaurantId);
            
            return new TestConnectionResult
            {
                IsSuccess = false,
                Message = $"خطا در اتصال: {ex.Message}",
                ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds
            };
        }
    }
}
