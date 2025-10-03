using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.AI;
using EazyMenu.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EazyMenu.Application.Features.AI.Commands.SaveAiSettings;

/// <summary>
/// هندلر ذخیره تنظیمات هوش مصنوعی
/// </summary>
public class SaveAiSettingsCommandHandler : IRequestHandler<SaveAiSettingsCommand, AiSettingsDto>
{
    private readonly IRepository<AiSettings> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SaveAiSettingsCommandHandler> _logger;

    public SaveAiSettingsCommandHandler(
        IRepository<AiSettings> repository,
        IUnitOfWork unitOfWork,
        ILogger<SaveAiSettingsCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AiSettingsDto> Handle(SaveAiSettingsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("ذخیره تنظیمات هوش مصنوعی برای رستوران {RestaurantId}", request.RestaurantId);

            // بررسی وجود تنظیمات قبلی
            var results = await _repository.FindAsync(
                x => x.RestaurantId == request.RestaurantId && !x.IsDeleted,
                cancellationToken);
            
            var existingSettings = results.FirstOrDefault();

            AiSettings settings;

            if (existingSettings != null)
            {
                // به‌روزرسانی تنظیمات موجود
                existingSettings.BaseUrl = request.BaseUrl;
                existingSettings.ApiKey = request.ApiKey;
                existingSettings.ModelName = request.ModelName;
                existingSettings.TimeoutSeconds = request.TimeoutSeconds;
                existingSettings.Environment = request.Environment;
                existingSettings.IsActive = true;

                await _repository.UpdateAsync(existingSettings, cancellationToken);
                settings = existingSettings;
            }
            else
            {
                // ایجاد تنظیمات جدید
                settings = new AiSettings
                {
                    RestaurantId = request.RestaurantId,
                    BaseUrl = request.BaseUrl,
                    ApiKey = request.ApiKey,
                    ModelName = request.ModelName,
                    TimeoutSeconds = request.TimeoutSeconds,
                    Environment = request.Environment,
                    IsActive = true
                };

                await _repository.AddAsync(settings, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("تنظیمات هوش مصنوعی با موفقیت ذخیره شد برای رستوران {RestaurantId}", request.RestaurantId);

            return new AiSettingsDto
            {
                Id = settings.Id,
                RestaurantId = settings.RestaurantId,
                BaseUrl = settings.BaseUrl,
                ApiKey = "***", // عدم نمایش کلید API کامل
                ModelName = settings.ModelName,
                TimeoutSeconds = settings.TimeoutSeconds,
                IsActive = settings.IsActive,
                Environment = settings.Environment
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ذخیره تنظیمات هوش مصنوعی برای رستوران {RestaurantId}", request.RestaurantId);
            throw;
        }
    }
}
