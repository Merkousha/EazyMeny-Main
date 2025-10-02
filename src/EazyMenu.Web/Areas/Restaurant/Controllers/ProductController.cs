using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EazyMenu.Web.Areas.Restaurant.Controllers;

/// <summary>
/// کنترلر مدیریت محصولات رستوران (منو)
/// </summary>
[Area("Restaurant")]
[Authorize(Roles = "RestaurantOwner")]
public class ProductController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(object model)
    {
        // TODO: Implement product creation
        TempData["Success"] = "محصول با موفقیت ایجاد شد";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(Guid id)
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, object model)
    {
        // TODO: Implement product edit
        TempData["Success"] = "محصول با موفقیت ویرایش شد";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(Guid id)
    {
        // TODO: Implement product deletion
        TempData["Success"] = "محصول با موفقیت حذف شد";
        return RedirectToAction(nameof(Index));
    }
}
