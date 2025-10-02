using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MediatR;
using EazyMenu.Application.Features.Restaurants.Queries.GetRestaurantByOwner;
using System.Security.Claims;

namespace EazyMenu.Web.Areas.Restaurant.Controllers;

[Area("Restaurant")]
[Authorize(Roles = "RestaurantOwner,Admin")]
public abstract class BaseRestaurantController : Controller
{
    protected readonly IMediator _mediator;

    protected BaseRestaurantController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// بارگذاری اطلاعات رستوران قبل از اجرای Action
    /// </summary>
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // دریافت ID کاربر
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var userGuid))
        {
            try
            {
                // دریافت اطلاعات رستوران کاربر
                var restaurant = await _mediator.Send(new GetRestaurantByOwnerQuery 
                { 
                    OwnerId = userGuid 
                });
                
                if (restaurant != null)
                {
                    // ذخیره در ViewData برای دسترسی در Layout
                    ViewData["RestaurantSlug"] = restaurant.Slug;
                    ViewData["RestaurantName"] = restaurant.Name;
                    ViewData["RestaurantId"] = restaurant.Id;
                }
                else
                {
                    // اگر رستورانی نداشت
                    ViewData["RestaurantSlug"] = string.Empty;
                    ViewData["RestaurantName"] = "رستوران";
                }
            }
            catch (Exception)
            {
                // در صورت خطا، مقادیر پیش‌فرض
                ViewData["RestaurantSlug"] = string.Empty;
                ViewData["RestaurantName"] = "رستوران";
            }
        }

        // اجرای Action
        await next();
    }

    /// <summary>
    /// دریافت RestaurantId از ViewData
    /// </summary>
    protected Guid GetRestaurantId()
    {
        if (ViewData["RestaurantId"] is Guid restaurantId)
        {
            return restaurantId;
        }
        
        throw new InvalidOperationException("RestaurantId not found in ViewData");
    }

    /// <summary>
    /// دریافت Restaurant Slug از ViewData
    /// </summary>
    protected string GetRestaurantSlug()
    {
        return ViewData["RestaurantSlug"]?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// دریافت Restaurant Name از ViewData
    /// </summary>
    protected string GetRestaurantName()
    {
        return ViewData["RestaurantName"]?.ToString() ?? string.Empty;
    }
}
