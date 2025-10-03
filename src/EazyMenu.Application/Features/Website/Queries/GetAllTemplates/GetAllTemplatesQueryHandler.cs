using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Features.Website.DTOs;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Website.Queries.GetAllTemplates;

/// <summary>
/// Handler دریافت تمام قالب‌ها
/// </summary>
public class GetAllTemplatesQueryHandler : IRequestHandler<GetAllTemplatesQuery, List<WebsiteTemplateDto>>
{
    private readonly IRepository<WebsiteTemplate> _templateRepository;
    private readonly IRepository<TemplateSection> _sectionRepository;

    public GetAllTemplatesQueryHandler(
        IRepository<WebsiteTemplate> templateRepository,
        IRepository<TemplateSection> sectionRepository)
    {
        _templateRepository = templateRepository;
        _sectionRepository = sectionRepository;
    }

    public async Task<List<WebsiteTemplateDto>> Handle(GetAllTemplatesQuery request, CancellationToken cancellationToken)
    {
        var templates = await _templateRepository.GetAllAsync(cancellationToken);
        var sections = await _sectionRepository.GetAllAsync(cancellationToken);

        // فیلتر کردن
        var filteredTemplates = templates.AsEnumerable();

        if (request.OnlyActive)
            filteredTemplates = filteredTemplates.Where(t => t.IsActive);

        if (request.OnlyFree)
            filteredTemplates = filteredTemplates.Where(t => t.IsFree);

        // مرتب‌سازی
        filteredTemplates = filteredTemplates.OrderBy(t => t.DisplayOrder).ThenBy(t => t.Name);

        // تبدیل به DTO
        var result = filteredTemplates.Select(t => new WebsiteTemplateDto
        {
            Id = t.Id,
            Name = t.Name,
            NameEn = t.NameEn,
            Description = t.Description,
            Type = t.Type,
            TemplatePath = t.TemplatePath,
            ThumbnailUrl = t.ThumbnailUrl,
            PreviewImageUrl = t.PreviewImageUrl,
            IsFree = t.IsFree,
            DisplayOrder = t.DisplayOrder,
            Sections = sections
                .Where(s => s.TemplateId == t.Id)
                .OrderBy(s => s.DisplayOrder)
                .Select(s => new TemplateSectionDto
                {
                    Id = s.Id,
                    SectionType = s.SectionType,
                    Title = s.Title,
                    TitleEn = s.TitleEn,
                    DefaultContent = s.DefaultContent,
                    IsRequired = s.IsRequired,
                    IsEditable = s.IsEditable,
                    DisplayOrder = s.DisplayOrder
                })
                .ToList()
        }).ToList();

        return result;
    }
}
