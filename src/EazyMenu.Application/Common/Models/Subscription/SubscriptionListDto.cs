namespace EazyMenu.Application.Common.Models.Subscription;

/// <summary>
/// DTO لیست اشتراک‌ها
/// </summary>
public class SubscriptionListDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public bool IsActive { get; set; }
    public int DaysRemaining { get; set; }
}
