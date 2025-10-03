using EazyMenu.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EazyMenu.Application.Features.AI.Commands.GenerateProductImage;

/// <summary>
/// هندلر تولید تصویر محصول با هوش مصنوعی
/// </summary>
public class GenerateProductImageCommandHandler : IRequestHandler<GenerateProductImageCommand, GenerateProductImageResult>
{
    private readonly IAiContentService _aiContentService;
    private readonly ILogger<GenerateProductImageCommandHandler> _logger;

    public GenerateProductImageCommandHandler(
        IAiContentService aiContentService,
        ILogger<GenerateProductImageCommandHandler> logger)
    {
        _aiContentService = aiContentService;
        _logger = logger;
    }

    public async Task<GenerateProductImageResult> Handle(GenerateProductImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("تولید تصویر برای محصول {ProductId} در رستوران {RestaurantId}", 
                request.ProductId, request.RestaurantId);

            var imageData = await _aiContentService.GenerateProductImageAsync(
                request.RestaurantId,
                request.Description,
                request.Style,
                request.Width,
                request.Height,
                cancellationToken);

            if (imageData != null && imageData.Length > 0)
            {
                _logger.LogInformation("تصویر با موفقیت تولید شد برای محصول {ProductId}. حجم: {Size} بایت", 
                    request.ProductId, imageData.Length);

                // ذخیره موقت تصویر
                var tempPath = Path.Combine(Path.GetTempPath(), $"ai-product-{request.ProductId}-{Guid.NewGuid()}.png");
                await File.WriteAllBytesAsync(tempPath, imageData, cancellationToken);

                return new GenerateProductImageResult
                {
                    IsSuccess = true,
                    ImageData = imageData,
                    TemporaryPath = tempPath
                };
            }
            else
            {
                _logger.LogWarning("تصویر خالی تولید شد برای محصول {ProductId}", request.ProductId);
                
                return new GenerateProductImageResult
                {
                    IsSuccess = false,
                    ErrorMessage = "تصویر تولید شده خالی است."
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای غیرمنتظره در تولید تصویر برای محصول {ProductId}", request.ProductId);
            
            return new GenerateProductImageResult
            {
                IsSuccess = false,
                ErrorMessage = "خطا در ارتباط با سرویس هوش مصنوعی. لطفاً بعداً تلاش کنید."
            };
        }
    }
}
