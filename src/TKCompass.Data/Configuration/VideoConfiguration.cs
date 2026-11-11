using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKCompass.Domain.Models;

namespace TKCompass.Data.Configuration;

public class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.ToTable("videos");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.VideoUrl).HasMaxLength(500);
        builder.Property(e => e.AdCode).HasMaxLength(100);
        builder.Property(e => e.Notes).HasMaxLength(1000);
        
        // 索引
        builder.HasIndex(e => e.CreatorId);
        builder.HasIndex(e => e.ShipmentId);
        builder.HasIndex(e => e.VideoDate);
        builder.HasIndex(e => e.Deleted);
        
        // 关系
        builder.HasOne(e => e.Creator)
            .WithMany()
            .HasForeignKey(e => e.CreatorId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(e => e.Shipment)
            .WithMany(e => e.Videos)
            .HasForeignKey(e => e.ShipmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
