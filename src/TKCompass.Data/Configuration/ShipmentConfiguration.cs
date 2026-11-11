using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKCompass.Domain.Models;

namespace TKCompass.Data.Configuration;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("shipments");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.TrackingNumber).HasMaxLength(100);
        builder.Property(e => e.Sku).HasMaxLength(500);
        builder.Property(e => e.ProductName).HasMaxLength(500);
        builder.Property(e => e.Notes).HasMaxLength(1000);
        
        // 索引
        builder.HasIndex(e => e.CreatorId);
        builder.HasIndex(e => e.SampleDate);
        builder.HasIndex(e => e.Deleted);
        
        // 关系
        builder.HasOne(e => e.Creator)
            .WithMany(e => e.Shipments)
            .HasForeignKey(e => e.CreatorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
