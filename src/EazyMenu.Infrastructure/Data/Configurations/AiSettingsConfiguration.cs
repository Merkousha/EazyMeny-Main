using EazyMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EazyMenu.Infrastructure.Data.Configurations;

/// <summary>
/// پیکربندی Entity تنظیمات هوش مصنوعی
/// </summary>
public class AiSettingsConfiguration : IEntityTypeConfiguration<AiSettings>
{
    public void Configure(EntityTypeBuilder<AiSettings> builder)
    {
        builder.ToTable("AiSettings");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.RestaurantId)
            .IsRequired();

        builder.Property(a => a.BaseUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.ApiKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.ModelName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.TimeoutSeconds)
            .HasDefaultValue(30);

        builder.Property(a => a.IsActive)
            .HasDefaultValue(true);

        builder.Property(a => a.Environment)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Production");

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(a => a.Restaurant)
            .WithMany()
            .HasForeignKey(a => a.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.RestaurantId)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(a => a.IsActive);
    }
}
