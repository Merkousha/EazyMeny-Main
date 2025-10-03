using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Features.Products.Queries.GetProductById;
using EazyMenu.Domain.Entities;
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
    private readonly IRepository<Product> _productRepository;

    public GenerateProductImageCommandHandler(
        IAiContentService aiContentService,
        ILogger<GenerateProductImageCommandHandler> logger,
                IRepository<Product> productRepository)
    {
        _aiContentService = aiContentService;
        _logger = logger;
        _productRepository = productRepository;

    }

    public async Task<GenerateProductImageResult> Handle(GenerateProductImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("تولید تصویر برای محصول {ProductId} در رستوران {RestaurantId}", 
                request.ProductId, request.RestaurantId);

            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

            var imageData = await _aiContentService.GenerateProductImageAsync(
                request.RestaurantId,
               product.Name,
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
