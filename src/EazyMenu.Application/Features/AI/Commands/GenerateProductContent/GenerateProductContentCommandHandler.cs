using EazyMenu.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EazyMenu.Application.Features.AI.Commands.GenerateProductContent;

/// <summary>
/// هندلر تولید محتوای محصول با هوش مصنوعی
/// </summary>
public class GenerateProductContentCommandHandler : IRequestHandler<GenerateProductContentCommand, AiGeneratedContentResult>
{
    private readonly IAiContentService _aiContentService;
    private readonly ILogger<GenerateProductContentCommandHandler> _logger;

    public GenerateProductContentCommandHandler(
        IAiContentService aiContentService,
        ILogger<GenerateProductContentCommandHandler> logger)
    {
        _aiContentService = aiContentService;
        _logger = logger;
    }

    public async Task<AiGeneratedContentResult> Handle(GenerateProductContentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("تولید محتوا برای محصول {ProductId} در رستوران {RestaurantId}", 
                request.ProductId, request.RestaurantId);

            var result = await _aiContentService.GenerateProductDescriptionAsync(
                request.RestaurantId,
                request.ProductName,
                request.Ingredients,
                request.Tone,
                cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("محتوا با موفقیت تولید شد برای محصول {ProductId}", request.ProductId);
            }
            else
            {
                _logger.LogWarning("خطا در تولید محتوا برای محصول {ProductId}: {Error}", 
                    request.ProductId, result.ErrorMessage);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای غیرمنتظره در تولید محتوا برای محصول {ProductId}", request.ProductId);
            
            return new AiGeneratedContentResult
            {
                IsSuccess = false,
                ErrorMessage = "خطا در ارتباط با سرویس هوش مصنوعی. لطفاً بعداً تلاش کنید."
            };
        }
    }
}
