using EazyMenu.Domain.Enums;

namespace EazyMenu.Application.Common.Models.Reservation;

/// <summary>
/// DTO کامل برای نمایش جزئیات رزرو
/// </summary>
public class ReservationDto
{
    public Guid Id { get; set; }
    public string ReservationNumber { get; set; } = string.Empty;
    
    // Restaurant
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string RestaurantPhone { get; set; } = string.Empty;
    public string RestaurantAddress { get; set; } = string.Empty;
    
    // Customer
    public Guid? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    
    // Reservation Details
    public DateTime ReservationDate { get; set; }
    public TimeSpan ReservationTime { get; set; }
    public DateTime ReservationDateTime => ReservationDate.Add(ReservationTime);
    public string FormattedDate => ReservationDate.ToString("yyyy/MM/dd");
    public string FormattedTime => ReservationTime.ToString(@"hh\:mm");
    public int GuestsCount { get; set; }
    
    // Status
    public ReservationStatus Status { get; set; }
    public string StatusDisplay => Status switch
    {
        ReservationStatus.Pending => "در انتظار تایید",
        ReservationStatus.Confirmed => "تایید شده",
        ReservationStatus.Cancelled => "لغو شده",
        ReservationStatus.Completed => "تکمیل شده",
        ReservationStatus.NoShow => "عدم حضور",
        _ => "نامشخص"
    };
    
    public string StatusBadgeClass => Status switch
    {
        ReservationStatus.Pending => "badge bg-warning",
        ReservationStatus.Confirmed => "badge bg-success",
        ReservationStatus.Cancelled => "badge bg-danger",
        ReservationStatus.Completed => "badge bg-primary",
        ReservationStatus.NoShow => "badge bg-secondary",
        _ => "badge bg-light"
    };
    
    // Additional Info
    public string? SpecialRequests { get; set; }
    public int? TableNumber { get; set; }
    public bool IsNoShow { get; set; }
    
    // Timestamps
    public DateTime? CheckedInAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public bool ReminderSent { get; set; }
    public DateTime? ReminderSentAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Computed Properties
    public bool IsPast => ReservationDateTime < DateTime.UtcNow;
    public bool IsToday => ReservationDate.Date == DateTime.UtcNow.Date;
    public bool IsTomorrow => ReservationDate.Date == DateTime.UtcNow.Date.AddDays(1);
    
    public string DaysUntilReservation
    {
        get
        {
            var days = (ReservationDate.Date - DateTime.UtcNow.Date).Days;
            if (days == 0) return "امروز";
            if (days == 1) return "فردا";
            if (days == -1) return "دیروز";
            if (days > 0) return $"{days} روز دیگر";
            return $"{Math.Abs(days)} روز پیش";
        }
    }
}
