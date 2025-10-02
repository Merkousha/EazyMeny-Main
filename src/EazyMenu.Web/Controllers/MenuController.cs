using EazyMenu.Application.Features.Menu.Queries.GetMenuBySlug;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EazyMenu.Web.Controllers;

/// <summary>
/// کنترلر منوی عمومی رستوران
/// </summary>
public class MenuController : Controller
{
    private readonly IMediator _mediator;

    public MenuController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// نمایش منوی رستوران - /menu/{slug}
    /// </summary>
    [HttpGet("/menu/{slug}")]
    public async Task<IActionResult> Index(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return NotFound();
        }

        var menu = await _mediator.Send(new GetMenuBySlugQuery(slug));

        if (menu == null)
        {
            ViewBag.ErrorMessage = "رستوران مورد نظر یافت نشد یا غیرفعال است.";
            return View("NotFound");
        }

        return View(menu);
    }
}
