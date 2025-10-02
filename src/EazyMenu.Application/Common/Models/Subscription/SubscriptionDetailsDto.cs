namespace EazyMenu.Application.Common.Models.Subscription;

/// <summary>
/// DTO جزئیات اشتراک
/// </summary>
public class SubscriptionDetailsDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string RestaurantPhone { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public bool AutoRenew { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public bool IsActive { get; set; }
    public int DaysRemaining { get; set; }
    public bool IsExpiringSoon => DaysRemaining > 0 && DaysRemaining <= 7;
}
