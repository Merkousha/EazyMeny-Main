using EazyMenu.Application.Features.Categories.Commands.CreateCategory;
using EazyMenu.Application.Features.Categories.Commands.DeleteCategory;
using EazyMenu.Application.Features.Categories.Commands.UpdateCategory;
using EazyMenu.Application.Features.Categories.Queries.GetAllCategories;
using EazyMenu.Application.Features.Categories.Queries.GetCategoryById;
using EazyMenu.Application.Features.Restaurants.Queries.GetAllRestaurants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EazyMenu.Web.Areas.Admin.Controllers;

/// <summary>
/// کنترلر مدیریت دسته‌بندی‌ها در پنل ادمین
/// </summary>
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoryController : Controller
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: Admin/Category
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "مدیریت دسته‌بندی‌ها";
        ViewData["PageHeader"] = "لیست دسته‌بندی‌ها";

        var categories = await _mediator.Send(new GetAllCategoriesQuery());
        return View(categories);
    }

    // GET: Admin/Category/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        var category = await _mediator.Send(new GetCategoryByIdQuery(id));

        if (category == null)
        {
            TempData["Error"] = "دسته‌بندی مورد نظر یافت نشد";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = $"جزئیات {category.Name}";
        ViewData["PageHeader"] = "جزئیات دسته‌بندی";

        return View(category);
    }

    // GET: Admin/Category/Create
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "افزودن دسته‌بندی";
        ViewData["PageHeader"] = "ایجاد دسته‌بندی جدید";

        await LoadRestaurantsDropdown();
        return View();
    }

    // POST: Admin/Category/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCategoryCommand command)
    {
        if (!ModelState.IsValid)
        {
            await LoadRestaurantsDropdown();
            return View(command);
        }

        try
        {
            var categoryId = await _mediator.Send(command);
            TempData["Success"] = "دسته‌بندی با موفقیت ایجاد شد";
            return RedirectToAction(nameof(Details), new { id = categoryId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            await LoadRestaurantsDropdown();
            return View(command);
        }
    }

    // GET: Admin/Category/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        var category = await _mediator.Send(new GetCategoryByIdQuery(id));

        if (category == null)
        {
            TempData["Error"] = "دسته‌بندی مورد نظر یافت نشد";
            return RedirectToAction(nameof(Index));
        }

        var command = new UpdateCategoryCommand
        {
            Id = category.Id,
            RestaurantId = category.RestaurantId,
            Name = category.Name,
            NameEn = category.NameEn,
            Description = category.Description,
            IconUrl = category.IconUrl,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive
        };

        ViewData["Title"] = $"ویرایش {category.Name}";
        ViewData["PageHeader"] = "ویرایش دسته‌بندی";

        await LoadRestaurantsDropdown(category.RestaurantId);
        return View(command);
    }

    // POST: Admin/Category/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateCategoryCommand command)
    {
        if (id != command.Id)
        {
            TempData["Error"] = "خطا در شناسایی دسته‌بندی";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            await LoadRestaurantsDropdown(command.RestaurantId);
            return View(command);
        }

        try
        {
            await _mediator.Send(command);
            TempData["Success"] = "دسته‌بندی با موفقیت ویرایش شد";
            return RedirectToAction(nameof(Details), new { id = command.Id });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            await LoadRestaurantsDropdown(command.RestaurantId);
            return View(command);
        }
    }

    // POST: Admin/Category/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new DeleteCategoryCommand(id));
            TempData["Success"] = "دسته‌بندی با موفقیت حذف شد";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// بارگذاری لیست رستوران‌ها برای Dropdown
    /// </summary>
    private async Task LoadRestaurantsDropdown(Guid? selectedRestaurantId = null)
    {
        var restaurants = await _mediator.Send(new GetAllRestaurantsQuery());
        ViewBag.Restaurants = new SelectList(restaurants, "Id", "Name", selectedRestaurantId);
    }
}
