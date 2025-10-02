using EazyMenu.Application.Common.Models.Cart;

namespace EazyMenu.Application.Common.Interfaces;

/// <summary>
/// سرویس مدیریت سبد خرید
/// </summary>
public interface ICartService
{
    /// <summary>
    /// دریافت سبد خرید فعلی
    /// </summary>
    CartDto GetCart();
    
    /// <summary>
    /// افزودن محصول به سبد
    /// </summary>
    Task<bool> AddToCartAsync(Guid restaurantId, string restaurantName, string restaurantSlug, 
        Guid productId, string productName, decimal price, decimal? discountedPrice, 
        string? imageUrl, int preparationTime, int quantity = 1, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// به‌روزرسانی تعداد محصول در سبد
    /// </summary>
    bool UpdateQuantity(Guid productId, int quantity);
    
    /// <summary>
    /// حذف محصول از سبد
    /// </summary>
    bool RemoveFromCart(Guid productId);
    
    /// <summary>
    /// خالی کردن سبد
    /// </summary>
    void ClearCart();
    
    /// <summary>
    /// تعداد آیتم‌های سبد
    /// </summary>
    int GetCartItemCount();
    
    /// <summary>
    /// بررسی خالی بودن سبد
    /// </summary>
    bool IsCartEmpty();
}
