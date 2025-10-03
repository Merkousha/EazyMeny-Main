using EazyMenu.Application.Common.Models.AI;
using MediatR;

namespace EazyMenu.Application.Features.AI.Queries.GetAiSettings;

/// <summary>
/// کوئری دریافت تنظیمات هوش مصنوعی
/// </summary>
public class GetAiSettingsQuery : IRequest<AiSettingsDto?>
{
    public Guid RestaurantId { get; set; }
}
