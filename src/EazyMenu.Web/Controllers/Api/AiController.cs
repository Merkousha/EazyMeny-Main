using EazyMenu.Application.Common.Models.AI;
using EazyMenu.Application.Features.AI.Commands.GenerateProductContent;
using EazyMenu.Application.Features.AI.Commands.GenerateProductImage;
using EazyMenu.Application.Features.AI.Commands.SaveAiSettings;
using EazyMenu.Application.Features.AI.Commands.TestAiConnection;
using EazyMenu.Application.Features.AI.Queries.GetAiSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EazyMenu.Web.Controllers.Api;

/// <summary>
/// API Controller برای هوش مصنوعی
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AiController> _logger;

    public AiController(IMediator mediator, ILogger<AiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// تولید محتوای محصول با هوش مصنوعی
    /// POST /api/ai/menu-items/{id}/generate-content
    /// </summary>
    [HttpPost("menu-items/{id}/generate-content")]
    public async Task<IActionResult> GenerateProductContent(
        Guid id,
        [FromBody] GenerateContentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            // TODO: دریافت RestaurantId از کاربر لاگین شده
            var restaurantId = Guid.Parse(User.FindFirst("RestaurantId")?.Value ?? Guid.Empty.ToString());

            var command = new GenerateProductContentCommand
            {
                RestaurantId = restaurantId,
                ProductId = id,
                ProductName = request.ProductName,
                Ingredients = request.Ingredients,
                Tone = request.Tone
            };

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        title = result.Title,
                        shortDescription = result.ShortDescription,
                        longDescription = result.LongDescription,
                        keywords = result.Keywords
                    }
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.ErrorMessage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در تولید محتوای محصول {ProductId}", id);
            
            return StatusCode(500, new
            {
                success = false,
                message = "خطا در تولید محتوا. لطفاً بعداً تلاش کنید."
            });
        }
    }

    /// <summary>
    /// تولید تصویر محصول با هوش مصنوعی
    /// POST /api/ai/menu-items/{id}/generate-image
    /// </summary>
    [HttpPost("menu-items/{id}/generate-image")]
    public async Task<IActionResult> GenerateProductImage(
        Guid id,
        [FromBody] GenerateImageRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var restaurantId = Guid.Parse(User.FindFirst("RestaurantId")?.Value ?? Guid.Empty.ToString());

            var command = new GenerateProductImageCommand
            {
                RestaurantId = restaurantId,
                ProductId = id,
                Description = request.Description,
                Style = request.Style,
                Width = request.Width,
                Height = request.Height
            };

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsSuccess && result.ImageData != null)
            {
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        imageUrl = result.TemporaryPath,
                        imageSize = result.ImageData.Length
                    }
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.ErrorMessage ?? "خطا در تولید تصویر"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در تولید تصویر محصول {ProductId}", id);
            
            return StatusCode(500, new
            {
                success = false,
                message = "خطا در تولید تصویر. لطفاً بعداً تلاش کنید."
            });
        }
    }

    /// <summary>
    /// دریافت تنظیمات هوش مصنوعی
    /// GET /api/ai/settings
    /// </summary>
    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings(CancellationToken cancellationToken)
    {
        try
        {
            var restaurantId = Guid.Parse(User.FindFirst("RestaurantId")?.Value ?? Guid.Empty.ToString());

            var query = new GetAiSettingsQuery { RestaurantId = restaurantId };
            var settings = await _mediator.Send(query, cancellationToken);

            if (settings != null)
            {
                return Ok(new
                {
                    success = true,
                    data = settings
                });
            }

            return NotFound(new
            {
                success = false,
                message = "تنظیمات هوش مصنوعی یافت نشد"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت تنظیمات AI");
            
            return StatusCode(500, new
            {
                success = false,
                message = "خطا در دریافت تنظیمات"
            });
        }
    }

    /// <summary>
    /// ذخیره تنظیمات هوش مصنوعی
    /// PUT /api/ai/settings
    /// </summary>
    [HttpPut("settings")]
    public async Task<IActionResult> SaveSettings(
        [FromBody] SaveAiSettingsCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var restaurantId = Guid.Parse(User.FindFirst("RestaurantId")?.Value ?? Guid.Empty.ToString());
            command.RestaurantId = restaurantId;

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new
            {
                success = true,
                data = result,
                message = "تنظیمات با موفقیت ذخیره شد"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ذخیره تنظیمات AI");
            
            return StatusCode(500, new
            {
                success = false,
                message = "خطا در ذخیره تنظیمات"
            });
        }
    }

    /// <summary>
    /// تست اتصال با سرویس هوش مصنوعی
    /// POST /api/ai/settings/test-connection
    /// </summary>
    [HttpPost("settings/test-connection")]
    public async Task<IActionResult> TestConnection(CancellationToken cancellationToken)
    {
        try
        {
            var restaurantId = Guid.Parse(User.FindFirst("RestaurantId")?.Value ?? Guid.Empty.ToString());

            var command = new TestAiConnectionCommand { RestaurantId = restaurantId };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new
            {
                success = result.IsSuccess,
                message = result.Message,
                responseTime = result.ResponseTimeMs
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در تست اتصال AI");
            
            return StatusCode(500, new
            {
                success = false,
                message = "خطا در تست اتصال"
            });
        }
    }
}
