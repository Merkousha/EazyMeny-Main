using EazyMenu.Application.Features.Website.DTOs;
using MediatR;

namespace EazyMenu.Application.Features.Website.Queries.GetAllTemplates;

/// <summary>
/// Query دریافت تمام قالب‌های وب‌سایت
/// </summary>
public class GetAllTemplatesQuery : IRequest<List<WebsiteTemplateDto>>
{
    public bool OnlyActive { get; set; } = true;
    public bool OnlyFree { get; set; } = false;
}
