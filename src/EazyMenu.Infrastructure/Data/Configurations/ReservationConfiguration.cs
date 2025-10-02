using EazyMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EazyMenu.Infrastructure.Data.Configurations;

/// <summary>
/// پیکربندی EF Core برای Entity رزرو میز
/// </summary>
public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        // Primary Key
        builder.HasKey(r => r.Id);
        
        // Properties
        builder.Property(r => r.ReservationNumber)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(r => r.CustomerName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(r => r.CustomerPhone)
            .IsRequired()
            .HasMaxLength(15);
        
        builder.Property(r => r.CustomerEmail)
            .HasMaxLength(100);
        
        builder.Property(r => r.ReservationDate)
            .IsRequired();
        
        builder.Property(r => r.ReservationTime)
            .IsRequired();
        
        builder.Property(r => r.GuestsCount)
            .IsRequired();
        
        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(r => r.SpecialRequests)
            .HasMaxLength(500);
        
        builder.Property(r => r.CancellationReason)
            .HasMaxLength(500);
        
        // Relationships
        builder.HasOne(r => r.Restaurant)
            .WithMany(rest => rest.Reservations)
            .HasForeignKey(r => r.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(r => r.Customer)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
        
        // Indexes
        builder.HasIndex(r => r.ReservationNumber)
            .IsUnique();
        
        builder.HasIndex(r => r.RestaurantId);
        
        builder.HasIndex(r => r.CustomerId);
        
        builder.HasIndex(r => r.ReservationDate);
        
        builder.HasIndex(r => r.Status);
        
        // Composite index for restaurant + date queries
        builder.HasIndex(r => new { r.RestaurantId, r.ReservationDate, r.Status });
    }
}
