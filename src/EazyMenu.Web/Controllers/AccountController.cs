using EazyMenu.Application.Common.Models.Auth;
using EazyMenu.Application.Features.Auth.Commands.Login;
using EazyMenu.Application.Features.Auth.Commands.Register;
using EazyMenu.Application.Features.Auth.Commands.SendOtp;
using EazyMenu.Application.Features.Auth.Commands.VerifyOtp;
using EazyMenu.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EazyMenu.Web.Controllers;

/// <summary>
/// کنترلر احراز هویت - ثبت‌نام، ورود، خروج، OTP
/// </summary>
public class AccountController : Controller
{
    private readonly IMediator _mediator;
    private readonly SignInManager<ApplicationIdentityUser> _signInManager;
    private readonly UserManager<ApplicationIdentityUser> _userManager;

    public AccountController(
        IMediator mediator,
        SignInManager<ApplicationIdentityUser> signInManager,
        UserManager<ApplicationIdentityUser> userManager)
    {
        _mediator = mediator;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    #region Register

    /// <summary>
    /// صفحه ثبت‌نام - GET
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// ثبت‌نام کاربر جدید - POST
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterDto model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // ارسال Command برای ثبت‌نام
        var command = new RegisterCommand
        {
            FullName = model.FullName,
            PhoneNumber = model.PhoneNumber,
            Email = model.Email,
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword,
            AcceptTerms = model.AcceptTerms
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "خطا در ثبت‌نام");
            return View(model);
        }

        // ورود خودکار بعد از ثبت‌نام
        var user = await _userManager.FindByNameAsync(model.PhoneNumber);
        if (user != null)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            TempData["Success"] = "ثبت‌نام با موفقیت انجام شد. خوش آمدید!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // هدایت ادمین به داشبورد
            if (await IsUserAdminAsync(user))
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            // هدایت صاحب رستوران به صفحه انتخاب پلن
            if (await IsUserRestaurantOwnerAsync(user))
            {
                return RedirectToAction("ChoosePlan", "Subscription");
            }

            return RedirectToAction("Index", "Home");
        }

        TempData["Success"] = "ثبت‌نام با موفقیت انجام شد. لطفاً وارد شوید.";
        return RedirectToAction(nameof(Login));
    }

    #endregion

    #region Login

    /// <summary>
    /// صفحه ورود - GET
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// ورود با رمز عبور - POST
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // ارسال Command برای ورود
        var command = new LoginCommand
        {
            PhoneOrEmail = model.PhoneOrEmail,
            Password = model.Password,
            RememberMe = model.RememberMe
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "خطا در ورود");
            return View(model);
        }

        // یافتن کاربر Identity برای SignIn
        ApplicationIdentityUser? user = null;
        
        if (model.PhoneOrEmail.Contains("@"))
        {
            user = await _userManager.FindByEmailAsync(model.PhoneOrEmail);
        }
        else
        {
            user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneOrEmail);
        }

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "خطا در ورود به سیستم");
            return View(model);
        }

        // SignIn با Identity
        await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);
        
        TempData["Success"] = $"خوش آمدید، {result.User!.FullName}!";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        // هدایت بر اساس نقش کاربر
        var roles = await _userManager.GetRolesAsync(user);
        
        if (roles.Contains("Admin"))
        {
            return Redirect("/Admin/Home/Index");
        }
        
        if (roles.Contains("RestaurantOwner"))
        {
            return Redirect("/Restaurant/Dashboard/Index");
        }

        // کاربر عادی به صفحه اصلی
        return RedirectToAction("Index", "Home");
    }

    #endregion

    #region OTP Login

    /// <summary>
    /// ارسال کد OTP - POST (AJAX)
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendOtp([FromBody] OtpRequestDto model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "شماره موبایل معتبر نیست" });
        }

        var command = new SendOtpCommand
        {
            PhoneNumber = model.PhoneNumber
        };

        var result = await _mediator.Send(command);

        return Json(new
        {
            success = result.Success,
            message = result.Message,
            expiresInSeconds = result.ExpiresInSeconds
        });
    }

    /// <summary>
    /// صفحه تایید OTP - GET
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult VerifyOtp(string phoneNumber, string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        if (string.IsNullOrEmpty(phoneNumber))
        {
            return RedirectToAction(nameof(Login));
        }

        ViewData["ReturnUrl"] = returnUrl;
        ViewData["PhoneNumber"] = phoneNumber;
        return View();
    }

    /// <summary>
    /// تایید کد OTP و ورود - POST
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyOtp(OtpVerifyDto model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        ViewData["PhoneNumber"] = model.PhoneNumber;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var command = new VerifyOtpCommand
        {
            PhoneNumber = model.PhoneNumber,
            Code = model.Code,
            RememberMe = model.RememberMe
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "خطا در تایید کد");
            return View(model);
        }

        // ورود کاربر با Identity
        var user = await _userManager.FindByNameAsync(result.User!.PhoneNumber);
        if (user == null)
        {
            // اگر کاربر Identity وجود نداشت، ایجاد می‌کنیم
            user = new ApplicationIdentityUser
            {
                Id = result.User.Id,
                UserName = result.User.PhoneNumber,
                PhoneNumber = result.User.PhoneNumber,
                Email = result.User.Email,
                PhoneNumberConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "خطا در ورود به سیستم");
                return View(model);
            }
        }

        await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);
        
        TempData["Success"] = $"خوش آمدید، {result.User.FullName}!";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        // هدایت ادمین به داشبورد
        if (await IsUserAdminAsync(user))
        {
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        return RedirectToAction("Index", "Home");
    }

    #endregion

    #region Logout

    /// <summary>
    /// خروج از سیستم - POST
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        TempData["Success"] = "با موفقیت خارج شدید";
        return RedirectToAction("Index", "Home");
    }

    #endregion

    #region Access Denied

    /// <summary>
    /// صفحه عدم دسترسی
    /// </summary>
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// بررسی نقش ادمین کاربر
    /// </summary>
    private async Task<bool> IsUserAdminAsync(ApplicationIdentityUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.Contains("Admin");
    }

    /// <summary>
    /// بررسی نقش صاحب رستوران
    /// </summary>
    private async Task<bool> IsUserRestaurantOwnerAsync(ApplicationIdentityUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.Contains("RestaurantOwner");
    }

    #endregion
}
