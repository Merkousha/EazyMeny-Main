using EazyMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EazyMenu.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration برای Entity رستوران
/// </summary>
public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.NameEn)
            .HasMaxLength(200);

        builder.Property(r => r.Slug)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(r => r.Slug)
            .IsUnique();

        builder.Property(r => r.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(r => r.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(r => r.Address)
            .IsRequired()
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(r => r.Owner)
            .WithMany(u => u.Restaurants)
            .HasForeignKey(r => r.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Subscription)
            .WithOne(s => s.Restaurant)
            .HasForeignKey<Subscription>(s => s.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Categories)
            .WithOne(c => c.Restaurant)
            .HasForeignKey(c => c.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Products)
            .WithOne(p => p.Restaurant)
            .HasForeignKey(p => p.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Orders)
            .WithOne(o => o.Restaurant)
            .HasForeignKey(o => o.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
