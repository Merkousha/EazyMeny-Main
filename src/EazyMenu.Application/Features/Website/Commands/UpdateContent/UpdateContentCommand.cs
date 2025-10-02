using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Website.Commands.UpdateContent;

/// <summary>
/// Command به‌روزرسانی محتوای یک بخش از وب‌سایت
/// </summary>
public class UpdateContentCommand : IRequest<bool>
{
    public Guid RestaurantId { get; set; }
    public SectionType SectionType { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool UseDefaultContent { get; set; }
    public bool IsVisible { get; set; } = true;
}
