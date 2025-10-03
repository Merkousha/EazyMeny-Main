using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.AI;
using EazyMenu.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EazyMenu.Application.Features.AI.Queries.GetAiSettings;

/// <summary>
/// هندلر دریافت تنظیمات هوش مصنوعی
/// </summary>
public class GetAiSettingsQueryHandler : IRequestHandler<GetAiSettingsQuery, AiSettingsDto?>
{
    private readonly IRepository<AiSettings> _repository;
    private readonly ILogger<GetAiSettingsQueryHandler> _logger;

    public GetAiSettingsQueryHandler(
        IRepository<AiSettings> repository,
        ILogger<GetAiSettingsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<AiSettingsDto?> Handle(GetAiSettingsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("دریافت تنظیمات هوش مصنوعی برای رستوران {RestaurantId}", request.RestaurantId);

            var results = await _repository.FindAsync(
                x => x.RestaurantId == request.RestaurantId && !x.IsDeleted,
                cancellationToken);
            
            var settings = results.FirstOrDefault();

            if (settings == null)
            {
                _logger.LogWarning("تنظیمات هوش مصنوعی برای رستوران {RestaurantId} یافت نشد", request.RestaurantId);
                return null;
            }

            return new AiSettingsDto
            {
                Id = settings.Id,
                RestaurantId = settings.RestaurantId,
                BaseUrl = settings.BaseUrl,
                ApiKey = MaskApiKey(settings.ApiKey),
                ModelName = settings.ModelName,
                TimeoutSeconds = settings.TimeoutSeconds,
                IsActive = settings.IsActive,
                Environment = settings.Environment
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت تنظیمات هوش مصنوعی برای رستوران {RestaurantId}", request.RestaurantId);
            throw;
        }
    }

    private string MaskApiKey(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey.Length < 8)
            return "***";

        return apiKey.Substring(0, 4) + "..." + apiKey.Substring(apiKey.Length - 4);
    }
}
