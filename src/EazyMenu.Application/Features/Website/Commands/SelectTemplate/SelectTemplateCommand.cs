using MediatR;

namespace EazyMenu.Application.Features.Website.Commands.SelectTemplate;

/// <summary>
/// Command انتخاب قالب برای رستوران
/// </summary>
public class SelectTemplateCommand : IRequest<bool>
{
    public Guid RestaurantId { get; set; }
    public Guid TemplateId { get; set; }
}
