using System.ComponentModel.DataAnnotations;

namespace EazyMenu.Application.Common.Models.Restaurant;

/// <summary>
/// DTO برای ایجاد رستوران جدید
/// </summary>
public class CreateRestaurantDto
{
    [Required(ErrorMessage = "نام رستوران الزامی است")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "نام رستوران باید بین 3 تا 200 کاراکتر باشد")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "توضیحات نباید بیش از 1000 کاراکتر باشد")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "آدرس الزامی است")]
    [StringLength(500, ErrorMessage = "آدرس نباید بیش از 500 کاراکتر باشد")]
    public string Address { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "نام شهر نباید بیش از 100 کاراکتر باشد")]
    public string? City { get; set; }

    [StringLength(100, ErrorMessage = "نام استان نباید بیش از 100 کاراکتر باشد")]
    public string? State { get; set; }

    [StringLength(20, ErrorMessage = "کد پستی نباید بیش از 20 کاراکتر باشد")]
    public string? PostalCode { get; set; }

    [Required(ErrorMessage = "شماره تلفن الزامی است")]
    [RegularExpression(@"^0\d{10}$", ErrorMessage = "شماره تلفن باید 11 رقم و با 0 شروع شود")]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست")]
    public string? Email { get; set; }

    [Url(ErrorMessage = "فرمت وب‌سایت صحیح نیست")]
    public string? Website { get; set; }

    // موقعیت جغرافیایی
    [Range(-90, 90, ErrorMessage = "عرض جغرافیایی باید بین -90 تا 90 باشد")]
    public double? Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "طول جغرافیایی باید بین -180 تا 180 باشد")]
    public double? Longitude { get; set; }

    // تنظیمات
    public bool IsActive { get; set; } = true;
    public bool AcceptsReservation { get; set; } = false;
    public bool AcceptsOnlineOrder { get; set; } = false;

    [Range(0, 1000, ErrorMessage = "تعداد میزها باید بین 0 تا 1000 باشد")]
    public int TableCount { get; set; } = 0;

    // هزینه‌ها
    [Range(0, 1000000, ErrorMessage = "هزینه ارسال باید بین 0 تا 1,000,000 تومان باشد")]
    public decimal DeliveryFee { get; set; } = 0;

    [Range(0, 10000000, ErrorMessage = "حداقل سفارش باید بین 0 تا 10,000,000 تومان باشد")]
    public decimal MinimumOrderAmount { get; set; } = 0;

    // ساعات کاری (JSON string)
    public string? WorkingHours { get; set; }
}
