using EazyMenu.Application.Common.Models.AI;
using MediatR;

namespace EazyMenu.Application.Features.AI.Commands.SaveAiSettings;

/// <summary>
/// دستور ذخیره تنظیمات هوش مصنوعی
/// </summary>
public class SaveAiSettingsCommand : IRequest<AiSettingsDto>
{
    public Guid RestaurantId { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public string Environment { get; set; } = "Production";
}
