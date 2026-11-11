using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKCompass.Domain.Models;

namespace TKCompass.Data.Configuration;

public class CreatorFulfillmentConfiguration : IEntityTypeConfiguration<CreatorFulfillmentDto>
{
    public void Configure(EntityTypeBuilder<CreatorFulfillmentDto> builder)
    {
        builder.ToView("v_creator_fulfillment");
        builder.HasNoKey();

        // 属性映射（视图列名 → DTO 属性）
        builder.Property(e => e.CreatorId).HasColumnName("creator_id");
        builder.Property(e => e.Handle).HasColumnName("handle");
        builder.Property(e => e.Nickname).HasColumnName("nickname");
        builder.Property(e => e.PreferredName).HasColumnName("preferred_name");
        builder.Property(e => e.QualityLevel).HasColumnName("quality_level");
        builder.Property(e => e.Tags).HasColumnName("tags");
        builder.Property(e => e.ShipmentId).HasColumnName("shipment_id");
        builder.Property(e => e.TrackingNumber).HasColumnName("tracking_number");
        builder.Property(e => e.Sku).HasColumnName("sku");
        builder.Property(e => e.SkuCode).HasColumnName("sku_code");
        builder.Property(e => e.SampleDate).HasColumnName("sample_date");
        builder.Property(e => e.ShipmentStatus).HasColumnName("shipment_status");
        builder.Property(e => e.RequiredVideoCount).HasColumnName("required_video_count");
        builder.Property(e => e.ValidVideoCount).HasColumnName("valid_video_count");
        builder.Property(e => e.MissingVideoCount).HasColumnName("missing_video_count");
        builder.Property(e => e.VideoNoCodeCount).HasColumnName("video_no_code_count");
        builder.Property(e => e.CodeCount).HasColumnName("code_count");
        builder.Property(e => e.TotalVideoCount).HasColumnName("total_video_count");
        builder.Property(e => e.CalculatedStatus).HasColumnName("calculated_status");
        builder.Property(e => e.HasMissingCode).HasColumnName("has_missing_code");
        builder.Property(e => e.TotalGmv).HasColumnName("total_gmv");
    }
}
