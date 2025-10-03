using MediatR;

namespace EazyMenu.Application.Features.AI.Commands.GenerateProductImage;

/// <summary>
/// دستور تولید تصویر محصول با هوش مصنوعی
/// </summary>
public class GenerateProductImageCommand : IRequest<GenerateProductImageResult>
{
    public Guid RestaurantId { get; set; }
    public Guid ProductId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Style { get; set; } = "واقعی";
    public int Width { get; set; } = 512;
    public int Height { get; set; } = 512;
}

/// <summary>
/// نتیجه تولید تصویر
/// </summary>
public class GenerateProductImageResult
{
    public byte[]? ImageData { get; set; }
    public string? TemporaryPath { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}
