using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKCompass.Domain.Models;

namespace TKCompass.Data.Configuration;

public class CreatorConfiguration : IEntityTypeConfiguration<Creator>
{
    public void Configure(EntityTypeBuilder<Creator> builder)
    {
        builder.ToTable("creators");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Handle).IsRequired().HasMaxLength(255);
        builder.Property(e => e.Nickname).IsRequired().HasMaxLength(255);
        builder.Property(e => e.QualityLevel).IsRequired().HasMaxLength(10);
        builder.Property(e => e.CooperationStatus).HasMaxLength(50);
        builder.Property(e => e.DeliveredReason).HasMaxLength(500);
        builder.Property(e => e.TiktokHomepageUrl).HasMaxLength(500);
        builder.Property(e => e.PreferredLanguage).HasMaxLength(50);
        builder.Property(e => e.ContactLocation).HasMaxLength(100);
        builder.Property(e => e.CountryRegion).HasMaxLength(100);
        builder.Property(e => e.ContentCategory).HasMaxLength(100);
        builder.Property(e => e.SourceChannel).HasMaxLength(100);
        builder.Property(e => e.CommissionRate).HasMaxLength(50);
        builder.Property(e => e.CreatorNotes).HasMaxLength(1000);
        
        // 索引
        builder.HasIndex(e => e.Handle).IsUnique();
        builder.HasIndex(e => e.QualityLevel);
        builder.HasIndex(e => e.CooperationStatus);
        builder.HasIndex(e => e.Deleted);
        builder.HasIndex(e => e.Delivered);
        builder.HasIndex(e => e.TodayStatus);
        builder.HasIndex(e => e.LatestContactDate);
        
        // 关系
        builder.HasOne(e => e.CreatorType)
            .WithMany()
            .HasForeignKey(e => e.CreatorTypeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
