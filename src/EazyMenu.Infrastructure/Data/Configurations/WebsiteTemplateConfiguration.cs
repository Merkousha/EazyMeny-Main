using EazyMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EazyMenu.Infrastructure.Data.Configurations;

/// <summary>
/// پیکربندی Entity Framework برای WebsiteTemplate
/// </summary>
public class WebsiteTemplateConfiguration : IEntityTypeConfiguration<WebsiteTemplate>
{
    public void Configure(EntityTypeBuilder<WebsiteTemplate> builder)
    {
        builder.HasKey(t => t.Id);
        
        // Properties
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(t => t.NameEn)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(t => t.Type)
            .IsRequired();
            
        builder.Property(t => t.TemplatePath)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(t => t.ThumbnailUrl)
            .IsRequired()
            .HasMaxLength(300);
            
        builder.Property(t => t.PreviewImageUrl)
            .IsRequired()
            .HasMaxLength(300);
            
        builder.Property(t => t.DefaultColors)
            .IsRequired()
            .HasMaxLength(1000);
            
        builder.Property(t => t.DefaultFonts)
            .IsRequired()
            .HasMaxLength(500);
            
        // Indexes
        builder.HasIndex(t => t.Type)
            .HasDatabaseName("IX_WebsiteTemplates_Type");
            
        builder.HasIndex(t => t.IsActive)
            .HasDatabaseName("IX_WebsiteTemplates_IsActive");
            
        builder.HasIndex(t => t.DisplayOrder)
            .HasDatabaseName("IX_WebsiteTemplates_DisplayOrder");
        
        // Relationships
        builder.HasMany(t => t.Sections)
            .WithOne(s => s.Template)
            .HasForeignKey(s => s.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
