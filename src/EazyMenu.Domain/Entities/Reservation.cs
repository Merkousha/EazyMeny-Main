using EazyMenu.Domain.Common;
using EazyMenu.Domain.Enums;

namespace EazyMenu.Domain.Entities;

/// <summary>
/// Entity رزرو میز
/// </summary>
public class Reservation : BaseEntity
{
    public string ReservationNumber { get; set; } = string.Empty; // مثلاً RES-12345
    
    public Guid RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;
    
    // مشتری
    public Guid? CustomerId { get; set; }
    public virtual ApplicationUser? Customer { get; set; }
    
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    
    // زمان رزرو
    public DateTime ReservationDate { get; set; }
    public TimeSpan ReservationTime { get; set; }
    public int GuestsCount { get; set; }
    
    // وضعیت
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    
    // جزئیات
    public string? SpecialRequests { get; set; }
    public int? TableNumber { get; set; }
    
    // حضور
    public DateTime? CheckedInAt { get; set; }
    public bool IsNoShow { get; set; } = false;
    
    // لغو
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    
    // یادآوری
    public bool ReminderSent { get; set; } = false;
    public DateTime? ReminderSentAt { get; set; }
}
