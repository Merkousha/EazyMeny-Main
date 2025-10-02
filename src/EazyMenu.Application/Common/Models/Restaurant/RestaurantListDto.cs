namespace EazyMenu.Application.Common.Models.Restaurant;

/// <summary>
/// DTO خلاصه برای لیست رستوران‌ها
/// </summary>
public class RestaurantListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool AcceptReservations { get; set; }
    public bool AcceptOnlineOrders { get; set; }
    public string QRCodeUrl { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
