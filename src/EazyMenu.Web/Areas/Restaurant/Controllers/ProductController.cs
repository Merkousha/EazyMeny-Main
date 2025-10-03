using EazyMenu.Application.Features.Products.Commands.CreateProduct;
using EazyMenu.Application.Features.Products.Commands.UpdateProduct;
using EazyMenu.Application.Features.Products.Commands.DeleteProduct;
using EazyMenu.Application.Features.Products.Queries.GetProductById;
using EazyMenu.Application.Features.Products.Queries.GetProductsByRestaurant;
using EazyMenu.Application.Features.Categories.Queries.GetCategoriesByRestaurant;
using EazyMenu.Application.Features.AI.Commands.GenerateProductContent;
using EazyMenu.Application.Features.AI.Commands.GenerateProductImage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EazyMenu.Web.Areas.Restaurant.Controllers;

/// <summary>
/// کنترلر مدیریت محصولات رستوران (منو)
/// </summary>
//[Area("Restaurant")]
//[Authorize(Roles = "RestaurantOwner")]
public class ProductController : BaseRestaurantController
{
    private readonly ILogger<ProductController> _logger;

    public ProductController(IMediator mediator, ILogger<ProductController> logger) 
        : base(mediator)
    {
        _logger = logger;
    }

    /// <summary>
    /// لیست محصولات
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var restaurantId = GetRestaurantId();
            var query = new GetProductsByRestaurantQuery(restaurantId);
            var products = await _mediator.Send(query);

            return View(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در بارگذاری لیست محصولات");
            TempData["Error"] = "خطا در بارگذاری لیست محصولات";
            return RedirectToAction("Index", "Dashboard");
        }
    }

    /// <summary>
    /// فرم ایجاد محصول جدید
    /// </summary>
    public async Task<IActionResult> Create()
    {
        await LoadCategories();
        return View();
    }

    /// <summary>
    /// ذخیره محصول جدید
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductCommand command)
    {
        try
        {
            command.RestaurantId = GetRestaurantId();

            if (!ModelState.IsValid)
            {
                await LoadCategories();
                return View(command);
            }

            var productId = await _mediator.Send(command);

            TempData["Success"] = "محصول با موفقیت ایجاد شد";
            return RedirectToAction(nameof(Edit), new { id = productId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ایجاد محصول");
            TempData["Error"] = "خطا در ایجاد محصول";
            await LoadCategories();
            return View(command);
        }
    }

    /// <summary>
    /// فرم ویرایش محصول
    /// </summary>
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var query = new GetProductByIdQuery(id);
            var product = await _mediator.Send(query);

            if (product == null)
            {
                TempData["Error"] = "محصول یافت نشد";
                return RedirectToAction(nameof(Index));
            }

            await LoadCategories();

            var command = new UpdateProductCommand
            {
                Id = product.Id,
                RestaurantId = product.RestaurantId,  // ✅ اضافه شد
                CategoryId = product.CategoryId,
                Name = product.Name,
                NameEn = product.NameEn,
                Description = product.Description,
                Price = product.Price,
                DiscountedPrice = product.DiscountedPrice,
                PreparationTime = product.PreparationTime,
                IsVegetarian = product.IsVegetarian,
                IsSpicy = product.IsSpicy,
                IsPopular = product.IsPopular,
                IsNew = product.IsNew,
                IsAvailable = product.IsAvailable,
                DisplayOrder = product.DisplayOrder,
                IsActive = product.IsActive,
                
                // فیلدهای اضافی
                Image1Url = product.Image1Url,
                Image2Url = product.Image2Url,
                Image3Url = product.Image3Url,
                StockQuantity = product.StockQuantity ?? 0,
                Options = product.Options,
                NutritionalInfo = product.NutritionalInfo
            };

            return View(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در بارگذاری محصول {ProductId}", id);
            TempData["Error"] = "خطا در بارگذاری محصول";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// ذخیره ویرایش محصول
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateProductCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                TempData["Error"] = "شناسه محصول نامعتبر است";
                return RedirectToAction(nameof(Index));
            }

            // ✅ امنیت: RestaurantId را از session بگیر، نه از فرم
            command.RestaurantId = GetRestaurantId();

            if (!ModelState.IsValid)
            {
                await LoadCategories();
                return View(command);
            }

            await _mediator.Send(command);

            TempData["Success"] = "محصول با موفقیت ویرایش شد";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ویرایش محصول {ProductId}", id);
            TempData["Error"] = "خطا در ویرایش محصول";
            await LoadCategories();
            return View(command);
        }
    }

    /// <summary>
    /// حذف محصول
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteProductCommand(id);
            await _mediator.Send(command);

            TempData["Success"] = "محصول با موفقیت حذف شد";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در حذف محصول {ProductId}", id);
            TempData["Error"] = "خطا در حذف محصول";
            return RedirectToAction(nameof(Index));
        }
    }

    #region AI Methods

    /// <summary>
    /// تولید محتوای محصول با AI
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> GenerateContent([FromBody] GenerateContentRequest request)
    {
        try
        {
            var restaurantId = GetRestaurantId();

            var command = new GenerateProductContentCommand
            {
                RestaurantId = restaurantId,
                ProductId = request.ProductId,
                ProductName = request.ProductName,
                Ingredients = request.Ingredients ?? string.Empty,
                Tone = request.Tone ?? "صمیمی"
            };

            var result = await _mediator.Send(command);

            return Json(new
            {
                success = result.IsSuccess,
                data = result.IsSuccess ? new
                {
                    title = result.Title,
                    shortDescription = result.ShortDescription,
                    longDescription = result.LongDescription,
                    keywords = result.Keywords
                } : null,
                message = result.IsSuccess ? "محتوا با موفقیت تولید شد" : result.ErrorMessage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در تولید محتوا با AI");
            return Json(new
            {
                success = false,
                message = "خطا در تولید محتوا. لطفاً بعداً تلاش کنید."
            });
        }
    }

    /// <summary>
    /// تولید تصویر محصول با AI
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> GenerateImage([FromBody] GenerateImageRequest request)
    {
        try
        {
            var restaurantId = GetRestaurantId();

            var command = new GenerateProductImageCommand
            {
                RestaurantId = restaurantId,
                ProductId = request.ProductId ?? Guid.Empty,
                Description = request.Description,
                Style = request.Style ?? "واقعی",
                Width = request.Width,
                Height = request.Height
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess && result.ImageData != null && result.ImageData.Length > 0)
            {
                // تبدیل به Base64 برای نمایش فوری در مرورگر
                var base64Image = Convert.ToBase64String(result.ImageData);
                var imageDataUrl = $"data:image/png;base64,{base64Image}";

                // ذخیره در wwwroot برای استفاده بعدی
                var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsFolder = Path.Combine(webRootPath, "images", "ai-generated", restaurantId.ToString());
                
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = $"{Guid.NewGuid()}.png";
                var filePath = Path.Combine(uploadsFolder, fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, result.ImageData);

                // URL قابل دسترسی از مرورگر
                var publicUrl = $"/images/ai-generated/{restaurantId}/{fileName}";

                _logger.LogInformation("تصویر با موفقیت تولید و ذخیره شد: {FilePath}", publicUrl);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        imageUrl = publicUrl,
                        imageDataUrl = imageDataUrl, // برای نمایش فوری
                        imageSize = result.ImageData.Length,
                        tempPath = result.TemporaryPath
                    },
                    message = "تصویر با موفقیت تولید شد"
                });
            }

            return Json(new
            {
                success = false,
                message = result.ErrorMessage ?? "خطا در تولید تصویر"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در تولید تصویر با AI");
            return Json(new
            {
                success = false,
                message = "خطا در تولید تصویر. لطفاً بعداً تلاش کنید."
            });
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// بارگذاری لیست دسته‌بندی‌ها برای Dropdown
    /// </summary>
    private async Task LoadCategories()
    {
        try
        {
            var restaurantId = GetRestaurantId();
            var query = new GetCategoriesByRestaurantQuery(restaurantId);
            var categories = await _mediator.Send(query);

            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در بارگذاری دسته‌بندی‌ها");
            ViewBag.Categories = new SelectList(Enumerable.Empty<SelectListItem>());
        }
    }

    #endregion
}

/// <summary>
/// درخواست تولید محتوا
/// </summary>
public class GenerateContentRequest
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Ingredients { get; set; }
    public string? Tone { get; set; }
}

/// <summary>
/// درخواست تولید تصویر
/// </summary>
public class GenerateImageRequest
{
    public Guid? ProductId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Style { get; set; }
    public int Width { get; set; } = 512;
    public int Height { get; set; } = 512;
}
