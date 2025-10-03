using EazyMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EazyMenu.Infrastructure.Data.Configurations;

/// <summary>
/// پیکربندی Entity سابقه چت
/// </summary>
public class ChatHistoryConfiguration : IEntityTypeConfiguration<ChatHistory>
{
    public void Configure(EntityTypeBuilder<ChatHistory> builder)
    {
        builder.ToTable("ChatHistories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.RestaurantId)
            .IsRequired();

        builder.Property(c => c.SessionId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.UserMessage)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.AiResponse)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.MessageTime)
            .IsRequired();

        builder.Property(c => c.IsUserMessage)
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(c => c.Restaurant)
            .WithMany()
            .HasForeignKey(c => c.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(c => c.RestaurantId);
        builder.HasIndex(c => c.SessionId);
        builder.HasIndex(c => c.MessageTime);
        builder.HasIndex(c => new { c.RestaurantId, c.SessionId, c.MessageTime });
    }
}
