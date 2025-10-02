using System.Text.Json;
using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Cart;
using Microsoft.AspNetCore.Http;

namespace EazyMenu.Infrastructure.Services;

/// <summary>
/// پیاده‌سازی Cart Service با استفاده از Session
/// </summary>
public class SessionCartService : ICartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CartSessionKey = "ShoppingCart";

    public SessionCartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CartDto GetCart()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null)
            return new CartDto();

        var cartJson = session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(cartJson))
            return new CartDto();

        return JsonSerializer.Deserialize<CartDto>(cartJson) ?? new CartDto();
    }

    public Task<bool> AddToCartAsync(
        Guid restaurantId, 
        string restaurantName, 
        string restaurantSlug,
        Guid productId, 
        string productName, 
        decimal price, 
        decimal? discountedPrice,
        string? imageUrl, 
        int preparationTime, 
        int quantity = 1,
        CancellationToken cancellationToken = default)
    {
        var cart = GetCart();

        // بررسی: آیا سبد خالی است یا از همان رستوران است؟
        if (cart.Items.Any() && cart.RestaurantId != restaurantId)
        {
            // سبد متعلق به رستوران دیگری است - نمی‌توان اضافه کرد
            return Task.FromResult(false);
        }

        // تنظیم اطلاعات رستوران
        if (!cart.Items.Any())
        {
            cart.RestaurantId = restaurantId;
            cart.RestaurantName = restaurantName;
            cart.RestaurantSlug = restaurantSlug;
        }

        // بررسی وجود محصول در سبد
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            cart.Items.Add(new CartItemDto
            {
                ProductId = productId,
                ProductName = productName,
                Price = price,
                DiscountedPrice = discountedPrice,
                Quantity = quantity,
                ImageUrl = imageUrl,
                PreparationTime = preparationTime
            });
        }

        SaveCart(cart);
        return Task.FromResult(true);
    }

    public bool UpdateQuantity(Guid productId, int quantity)
    {
        var cart = GetCart();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        
        if (item == null)
            return false;

        if (quantity <= 0)
        {
            cart.Items.Remove(item);
        }
        else
        {
            item.Quantity = quantity;
        }

        SaveCart(cart);
        return true;
    }

    public bool RemoveFromCart(Guid productId)
    {
        var cart = GetCart();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        
        if (item == null)
            return false;

        cart.Items.Remove(item);
        SaveCart(cart);
        return true;
    }

    public void ClearCart()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        session?.Remove(CartSessionKey);
    }

    public int GetCartItemCount()
    {
        var cart = GetCart();
        return cart.TotalItems;
    }

    public bool IsCartEmpty()
    {
        var cart = GetCart();
        return !cart.Items.Any();
    }

    private void SaveCart(CartDto cart)
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null)
            return;

        var cartJson = JsonSerializer.Serialize(cart);
        session.SetString(CartSessionKey, cartJson);
    }
}
