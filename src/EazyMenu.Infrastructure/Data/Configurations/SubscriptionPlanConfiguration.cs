using EazyMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EazyMenu.Infrastructure.Data.Configurations;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.HasKey(sp => sp.Id);
        
        builder.Property(sp => sp.PlanType)
            .IsRequired();
            
        builder.Property(sp => sp.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(sp => sp.Description)
            .HasMaxLength(500);
            
        builder.Property(sp => sp.PriceMonthly)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
            
        builder.Property(sp => sp.PriceYearly)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
            
        builder.Property(sp => sp.SupportLevel)
            .HasMaxLength(100);
            
        builder.Property(sp => sp.Features)
            .HasColumnType("nvarchar(max)");
            
        builder.HasIndex(sp => sp.PlanType)
            .IsUnique();
            
        builder.HasIndex(sp => sp.DisplayOrder);
        
        // Relationships
        builder.HasMany(sp => sp.Subscriptions)
            .WithOne(s => s.SubscriptionPlan)
            .HasForeignKey(s => s.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
