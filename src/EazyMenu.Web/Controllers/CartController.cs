using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EazyMenu.Web.Controllers;

/// <summary>
/// کنترلر مدیریت سبد خرید
/// </summary>
public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;

    public CartController(
        ICartService cartService,
        IRepository<Product> productRepository,
        IRepository<Restaurant> restaurantRepository)
    {
        _cartService = cartService;
        _productRepository = productRepository;
        _restaurantRepository = restaurantRepository;
    }

    /// <summary>
    /// نمایش صفحه سبد خرید
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        var cart = _cartService.GetCart();
        return View(cart);
    }

    /// <summary>
    /// افزودن محصول به سبد خرید (Ajax)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddToCart(
        Guid productId,
        int quantity = 1)
    {
        try
        {
            // دریافت محصول
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "محصول مورد نظر یافت نشد." });
            }

            // بررسی موجودی
            if (!product.IsAvailable)
            {
                return Json(new { success = false, message = "این محصول در حال حاضر موجود نیست." });
            }

            // دریافت اطلاعات رستوران
            var restaurant = await _restaurantRepository.GetByIdAsync(product.RestaurantId);
            if (restaurant == null)
            {
                return Json(new { success = false, message = "رستوران یافت نشد." });
            }

            // افزودن به سبد خرید
            var success = await _cartService.AddToCartAsync(
                restaurantId: restaurant.Id,
                restaurantName: restaurant.Name,
                restaurantSlug: restaurant.Slug,
                productId: product.Id,
                productName: product.Name,
                price: product.Price,
                discountedPrice: product.DiscountedPrice,
                imageUrl: product.Image1Url ?? "/images/no-image.png",
                preparationTime: product.PreparationTime,
                quantity: quantity
            );

            if (!success)
            {
                return Json(new
                {
                    success = false,
                    message = $"شما نمی‌توانید محصولات رستوران‌های مختلف را در یک سبد داشته باشید. لطفا ابتدا سبد خرید فعلی را تکمیل کنید یا آن را خالی کنید."
                });
            }

            // دریافت تعداد کل آیتم‌های سبد خرید
            var cartItemCount = _cartService.GetCartItemCount();

            return Json(new
            {
                success = true,
                message = $"{product.Name} به سبد خرید اضافه شد.",
                cartItemCount = cartItemCount
            });
        }
        catch (Exception)
        {
            return Json(new
            {
                success = false,
                message = "خطایی در افزودن محصول به سبد خرید رخ داد. لطفا دوباره تلاش کنید."
            });
        }
    }

    /// <summary>
    /// به‌روزرسانی تعداد محصول در سبد خرید (Ajax)
    /// </summary>
    [HttpPost]
    public IActionResult UpdateQuantity(Guid productId, int quantity)
    {
        try
        {
            if (quantity < 1)
            {
                return Json(new { success = false, message = "تعداد باید حداقل ۱ باشد." });
            }

            if (quantity > 99)
            {
                return Json(new { success = false, message = "حداکثر تعداد مجاز ۹۹ عدد است." });
            }

            var success = _cartService.UpdateQuantity(productId, quantity);
            if (!success)
            {
                return Json(new { success = false, message = "محصول در سبد خرید یافت نشد." });
            }

            var cart = _cartService.GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            return Json(new
            {
                success = true,
                message = "تعداد محصول به‌روزرسانی شد.",
                subtotal = item?.Subtotal ?? 0,
                totalAmount = cart.TotalAmount,
                totalDiscount = cart.TotalDiscount,
                cartItemCount = cart.TotalItems
            });
        }
        catch (Exception)
        {
            return Json(new
            {
                success = false,
                message = "خطایی در به‌روزرسانی تعداد رخ داد."
            });
        }
    }

    /// <summary>
    /// حذف محصول از سبد خرید (Ajax)
    /// </summary>
    [HttpPost]
    public IActionResult RemoveFromCart(Guid productId)
    {
        try
        {
            var success = _cartService.RemoveFromCart(productId);
            if (!success)
            {
                return Json(new { success = false, message = "محصول در سبد خرید یافت نشد." });
            }

            var cart = _cartService.GetCart();

            return Json(new
            {
                success = true,
                message = "محصول از سبد خرید حذف شد.",
                totalAmount = cart.TotalAmount,
                totalDiscount = cart.TotalDiscount,
                cartItemCount = cart.TotalItems,
                isEmpty = _cartService.IsCartEmpty()
            });
        }
        catch (Exception)
        {
            return Json(new
            {
                success = false,
                message = "خطایی در حذف محصول رخ داد."
            });
        }
    }

    /// <summary>
    /// خالی کردن سبد خرید
    /// </summary>
    [HttpPost]
    public IActionResult ClearCart()
    {
        try
        {
            _cartService.ClearCart();
            return Json(new
            {
                success = true,
                message = "سبد خرید خالی شد."
            });
        }
        catch (Exception)
        {
            return Json(new
            {
                success = false,
                message = "خطایی در خالی کردن سبد خرید رخ داد."
            });
        }
    }

    /// <summary>
    /// دریافت تعداد آیتم‌های سبد خرید (Ajax)
    /// </summary>
    [HttpGet]
    public IActionResult GetCartCount()
    {
        var count = _cartService.GetCartItemCount();
        return Json(new { count = count });
    }
}
