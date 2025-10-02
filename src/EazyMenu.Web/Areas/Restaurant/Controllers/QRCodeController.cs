using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;

namespace EazyMenu.Web.Areas.Restaurant.Controllers;

/// <summary>
/// کنترلر مدیریت QR Code رستوران
/// </summary>
[Area("Restaurant")]
[Authorize(Roles = "RestaurantOwner")]
public class QRCodeController : Controller
{
    private readonly IRepository<Domain.Entities.Restaurant> _restaurantRepository;
    private readonly IQRCodeService _qrCodeService;

    public QRCodeController(
        IRepository<Domain.Entities.Restaurant> restaurantRepository,
        IQRCodeService qrCodeService)
    {
        _restaurantRepository = restaurantRepository;
        _qrCodeService = qrCodeService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var ownerGuid))
        {
            TempData["Error"] = "کاربر یافت نشد";
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        var restaurants = await _restaurantRepository.FindAsync(
            r => r.OwnerId == ownerGuid && !r.IsDeleted,
            CancellationToken.None);

        var restaurant = restaurants.FirstOrDefault();
        if (restaurant == null)
        {
            TempData["Error"] = "رستورانی یافت نشد";
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        ViewBag.RestaurantName = restaurant.Name;
        ViewBag.RestaurantSlug = restaurant.Slug;
        ViewBag.QRCodeUrl = restaurant.QRCodeUrl;
        ViewBag.MenuUrl = $"{Request.Scheme}://{Request.Host}/menu/{restaurant.Slug}";

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Regenerate()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var ownerGuid))
        {
            TempData["Error"] = "کاربر یافت نشد";
            return RedirectToAction(nameof(Index));
        }

        var restaurants = await _restaurantRepository.FindAsync(
            r => r.OwnerId == ownerGuid && !r.IsDeleted,
            CancellationToken.None);

        var restaurant = restaurants.FirstOrDefault();
        if (restaurant == null)
        {
            TempData["Error"] = "رستورانی یافت نشد";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var menuUrl = $"{Request.Scheme}://{Request.Host}/menu/{restaurant.Slug}";
            var qrCodePath = await _qrCodeService.SaveQRCodeAsync(restaurant.Slug, menuUrl, 500);
            
            restaurant.QRCodeUrl = qrCodePath;
            await _restaurantRepository.UpdateAsync(restaurant, CancellationToken.None);

            TempData["Success"] = "QR Code با موفقیت ساخته شد";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"خطا در ساخت QR Code: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Download()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var ownerGuid))
        {
            return NotFound();
        }

        var restaurants = await _restaurantRepository.FindAsync(
            r => r.OwnerId == ownerGuid && !r.IsDeleted,
            CancellationToken.None);

        var restaurant = restaurants.FirstOrDefault();
        if (restaurant == null)
        {
            return NotFound();
        }

        if (string.IsNullOrEmpty(restaurant.QRCodeUrl))
        {
            TempData["Error"] = "QR Code یافت نشد. لطفاً ابتدا QR Code را بسازید";
            return RedirectToAction(nameof(Index));
        }

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", restaurant.QRCodeUrl.TrimStart('/'));
        
        if (!System.IO.File.Exists(filePath))
        {
            TempData["Error"] = "فایل QR Code یافت نشد";
            return RedirectToAction(nameof(Index));
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(fileBytes, "image/png", $"qrcode-{restaurant.Slug}.png");
    }
}
