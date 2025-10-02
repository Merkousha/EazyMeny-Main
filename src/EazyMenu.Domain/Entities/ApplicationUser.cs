using EazyMenu.Domain.Common;

namespace EazyMenu.Domain.Entities;

/// <summary>
/// کاربر سیستم - Base Entity بدون وابستگی به Identity
/// </summary>
public class ApplicationUser : BaseEntity
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; } = false;
    public bool PhoneNumberConfirmed { get; set; } = false;
    
    // اطلاعات تکمیلی (اختیاری)
    public string? ProfileImageUrl { get; set; }
    public string PreferredLanguage { get; set; } = "fa"; // fa or en
    
    // Relationships
    public virtual ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
