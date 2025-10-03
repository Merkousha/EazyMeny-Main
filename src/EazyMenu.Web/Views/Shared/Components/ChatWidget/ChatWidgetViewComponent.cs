using Microsoft.AspNetCore.Mvc;

namespace EazyMenu.Web.Views.Shared.Components.ChatWidget;

/// <summary>
/// View Component برای ویجت چت هوش مصنوعی
/// </summary>
public class ChatWidgetViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Guid restaurantId)
    {
        return View(restaurantId);
    }
}
