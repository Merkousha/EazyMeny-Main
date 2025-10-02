using EazyMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EazyMenu.Infrastructure.Data.Configurations;

/// <summary>
/// پیکربندی Entity Framework برای WebsiteCustomization
/// </summary>
public class WebsiteCustomizationConfiguration : IEntityTypeConfiguration<WebsiteCustomization>
{
    public void Configure(EntityTypeBuilder<WebsiteCustomization> builder)
    {
        builder.HasKey(c => c.Id);
        
        // Properties
        builder.Property(c => c.PrimaryColor)
            .IsRequired()
            .HasMaxLength(7); // #RRGGBB
            
        builder.Property(c => c.SecondaryColor)
            .IsRequired()
            .HasMaxLength(7);
            
        builder.Property(c => c.TextColor)
            .IsRequired()
            .HasMaxLength(7);
            
        builder.Property(c => c.BackgroundColor)
            .IsRequired()
            .HasMaxLength(7);
            
        builder.Property(c => c.FontFamily)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(c => c.CustomLogoUrl)
            .HasMaxLength(300);
            
        builder.Property(c => c.FaviconUrl)
            .HasMaxLength(300);
            
        builder.Property(c => c.SeoTitle)
            .HasMaxLength(100);
            
        builder.Property(c => c.SeoDescription)
            .HasMaxLength(300);
            
        builder.Property(c => c.SeoKeywords)
            .HasMaxLength(200);
            
        builder.Property(c => c.SocialImageUrl)
            .HasMaxLength(300);
            
        builder.Property(c => c.GoogleAnalyticsId)
            .HasMaxLength(50);
            
        builder.Property(c => c.CustomCss)
            .HasMaxLength(10000);
            
        builder.Property(c => c.CustomJs)
            .HasMaxLength(10000);
        
        // Indexes
        builder.HasIndex(c => c.RestaurantId)
            .IsUnique()
            .HasDatabaseName("IX_WebsiteCustomizations_RestaurantId");
        
        // Relationships
        builder.HasOne(c => c.Restaurant)
            .WithOne(r => r.WebsiteCustomization)
            .HasForeignKey<WebsiteCustomization>(c => c.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
