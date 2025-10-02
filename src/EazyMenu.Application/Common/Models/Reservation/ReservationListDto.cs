using EazyMenu.Domain.Enums;

namespace EazyMenu.Application.Common.Models.Reservation;

/// <summary>
/// DTO ساده برای لیست رزروها
/// </summary>
public class ReservationListDto
{
    public Guid Id { get; set; }
    public string ReservationNumber { get; set; } = string.Empty;
    
    public string RestaurantName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    
    public DateTime ReservationDate { get; set; }
    public TimeSpan ReservationTime { get; set; }
    public string FormattedDate => ReservationDate.ToString("yyyy/MM/dd");
    public string FormattedTime => ReservationTime.ToString(@"hh\:mm");
    public string FormattedDateTime => $"{FormattedDate} - {FormattedTime}";
    
    public int GuestsCount { get; set; }
    public int? TableNumber { get; set; }
    
    public ReservationStatus Status { get; set; }
    public string StatusDisplay => Status switch
    {
        ReservationStatus.Pending => "در انتظار",
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
    
    public DateTime CreatedAt { get; set; }
    
    // Helper
    public bool IsPast => ReservationDate.Add(ReservationTime) < DateTime.UtcNow;
    public bool IsToday => ReservationDate.Date == DateTime.UtcNow.Date;
}
