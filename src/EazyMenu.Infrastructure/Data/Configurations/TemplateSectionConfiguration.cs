using EazyMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EazyMenu.Infrastructure.Data.Configurations;

/// <summary>
/// پیکربندی Entity Framework برای TemplateSection
/// </summary>
public class TemplateSectionConfiguration : IEntityTypeConfiguration<TemplateSection>
{
    public void Configure(EntityTypeBuilder<TemplateSection> builder)
    {
        builder.HasKey(s => s.Id);
        
        // Properties
        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(s => s.TitleEn)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(s => s.SectionType)
            .IsRequired();
            
        builder.Property(s => s.DefaultContent)
            .IsRequired()
            .HasMaxLength(10000);
        
        // Indexes
        builder.HasIndex(s => new { s.TemplateId, s.SectionType })
            .HasDatabaseName("IX_TemplateSections_TemplateId_SectionType");
            
        builder.HasIndex(s => s.DisplayOrder)
            .HasDatabaseName("IX_TemplateSections_DisplayOrder");
        
        // Relationships (در WebsiteTemplateConfiguration تعریف شده)
    }
}
