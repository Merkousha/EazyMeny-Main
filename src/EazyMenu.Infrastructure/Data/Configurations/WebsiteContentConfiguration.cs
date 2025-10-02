using EazyMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EazyMenu.Infrastructure.Data.Configurations;

/// <summary>
/// پیکربندی Entity Framework برای WebsiteContent
/// </summary>
public class WebsiteContentConfiguration : IEntityTypeConfiguration<WebsiteContent>
{
    public void Configure(EntityTypeBuilder<WebsiteContent> builder)
    {
        builder.HasKey(c => c.Id);
        
        // Properties
        builder.Property(c => c.SectionType)
            .IsRequired();
            
        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(20000);
        
        // Indexes
        builder.HasIndex(c => c.RestaurantId)
            .HasDatabaseName("IX_WebsiteContents_RestaurantId");
            
        builder.HasIndex(c => new { c.RestaurantId, c.SectionType })
            .HasDatabaseName("IX_WebsiteContents_RestaurantId_SectionType");
            
        builder.HasIndex(c => c.TemplateId)
            .HasDatabaseName("IX_WebsiteContents_TemplateId");
        
        // Relationships
        builder.HasOne(c => c.Restaurant)
            .WithMany(r => r.WebsiteContents)
            .HasForeignKey(c => c.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(c => c.Template)
            .WithMany()
            .HasForeignKey(c => c.TemplateId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
