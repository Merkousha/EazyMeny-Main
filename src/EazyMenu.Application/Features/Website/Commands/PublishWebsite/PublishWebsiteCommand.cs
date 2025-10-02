using MediatR;

namespace EazyMenu.Application.Features.Website.Commands.PublishWebsite;

/// <summary>
/// Command انتشار یا عدم انتشار وب‌سایت
/// </summary>
public class PublishWebsiteCommand : IRequest<bool>
{
    public Guid RestaurantId { get; set; }
    public bool IsPublished { get; set; }
}
