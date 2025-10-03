using EazyMenu.Application.Common.Models.Product;
using EazyMenu.Application.Features.Products.Commands.CreateProduct;
using EazyMenu.Application.Features.Products.Commands.DeleteProduct;
using EazyMenu.Application.Features.Products.Commands.UpdateProduct;
using EazyMenu.Application.Features.Products.Queries.GetAllProducts;
using EazyMenu.Application.Features.Products.Queries.GetProductById;
using EazyMenu.Application.Features.Restaurants.Queries.GetAllRestaurants;
using EazyMenu.Application.Features.Categories.Queries.GetCategoriesByRestaurant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EazyMenu.Web.Areas.Admin.Controllers;

/// <summary>
/// کنترلر مدیریت محصولات در پنل ادمین
/// </summary>
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductController : Controller
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// صفحه لیست محصولات
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var products = await _mediator.Send(new GetAllProductsQuery());
        return View(products);
    }

    /// <summary>
    /// صفحه جزئیات محصول
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        
        if (product == null)
        {
            TempData["Error"] = "محصول مورد نظر یافت نشد.";
            return RedirectToAction(nameof(Index));
        }
        
        return View(product);
    }

    /// <summary>
    /// صفحه ایجاد محصول جدید
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadRestaurantsDropdown();
        return View();
    }

    /// <summary>
    /// ثبت محصول جدید
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductCommand command)
    {
        if (!ModelState.IsValid)
        {
            await LoadRestaurantsDropdown();
            await LoadCategoriesDropdown(command.RestaurantId);
            return View(command);
        }

        try
        {
            var productId = await _mediator.Send(command);
            TempData["Success"] = "محصول با موفقیت ایجاد شد.";
            return RedirectToAction(nameof(Details), new { id = productId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"خطا در ایجاد محصول: {ex.Message}";
            await LoadRestaurantsDropdown();
            await LoadCategoriesDropdown(command.RestaurantId);
            return View(command);
        }
    }

    /// <summary>
    /// صفحه ویرایش محصول
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        
        if (product == null)
        {
            TempData["Error"] = "محصول مورد نظر یافت نشد.";
            return RedirectToAction(nameof(Index));
        }

        var command = new UpdateProductCommand
        {
            Id = product.Id,
            RestaurantId = product.RestaurantId,
            CategoryId = product.CategoryId,
            Name = product.Name,
            NameEn = product.NameEn,
            Description = product.Description,
            Price = product.Price,
            DiscountedPrice = product.DiscountedPrice,
            Image1Url = product.Image1Url,
            Image2Url = product.Image2Url,
            Image3Url = product.Image3Url,
            DisplayOrder = product.DisplayOrder,
            IsActive = product.IsActive,
            IsAvailable = product.IsAvailable,
            IsNew = product.IsNew,
            IsPopular = product.IsPopular,
            IsSpicy = product.IsSpicy,
            IsVegetarian = product.IsVegetarian,    
            StockQuantity = product.StockQuantity ?? 0,
            PreparationTime = product.PreparationTime,
            Options = product.Options,
            NutritionalInfo = product.NutritionalInfo
        };

        await LoadRestaurantsDropdown();
        await LoadCategoriesDropdown(product.RestaurantId);
        
        return View(command);
    }

    /// <summary>
    /// ویرایش محصول
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateProductCommand command)
    {
        if (!ModelState.IsValid)
        {
            await LoadRestaurantsDropdown();
            await LoadCategoriesDropdown(command.RestaurantId);
            return View(command);
        }

        try
        {
            await _mediator.Send(command);
            TempData["Success"] = "محصول با موفقیت ویرایش شد.";
            return RedirectToAction(nameof(Details), new { id = command.Id });
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"خطا در ویرایش محصول: {ex.Message}";
            await LoadRestaurantsDropdown();
            await LoadCategoriesDropdown(command.RestaurantId);
            return View(command);
        }
    }

    /// <summary>
    /// حذف محصول (Soft Delete)
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new DeleteProductCommand(id));
            TempData["Success"] = "محصول با موفقیت حذف شد.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"خطا در حذف محصول: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// دریافت دسته‌بندی‌های یک رستوران (Ajax)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCategoriesByRestaurant(Guid restaurantId)
    {
        var categories = await _mediator.Send(new GetCategoriesByRestaurantQuery(restaurantId));
        return Json(categories.Select(c => new { value = c.Id, text = c.Name }));
    }

    #region Helper Methods

    /// <summary>
    /// بارگذاری لیست رستوران‌ها برای Dropdown
    /// </summary>
    private async Task LoadRestaurantsDropdown()
    {
        var restaurants = await _mediator.Send(new GetAllRestaurantsQuery());
        ViewBag.Restaurants = new SelectList(restaurants, "Id", "Name");
    }

    /// <summary>
    /// بارگذاری لیست دسته‌بندی‌ها برای Dropdown
    /// </summary>
    private async Task LoadCategoriesDropdown(Guid? restaurantId)
    {
        if (restaurantId.HasValue && restaurantId.Value != Guid.Empty)
        {
            var categories = await _mediator.Send(new GetCategoriesByRestaurantQuery(restaurantId.Value));
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }
        else
        {
            ViewBag.Categories = new SelectList(Enumerable.Empty<object>(), "Id", "Name");
        }
    }

    #endregion
}
