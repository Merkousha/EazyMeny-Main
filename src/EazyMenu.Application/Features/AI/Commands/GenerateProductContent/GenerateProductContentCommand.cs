using EazyMenu.Application.Common.Interfaces;
using MediatR;

namespace EazyMenu.Application.Features.AI.Commands.GenerateProductContent;

/// <summary>
/// دستور تولید محتوای محصول با هوش مصنوعی
/// </summary>
public class GenerateProductContentCommand : IRequest<AiGeneratedContentResult>
{
    public Guid RestaurantId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty;
    public string Tone { get; set; } = "صمیمی";
}
