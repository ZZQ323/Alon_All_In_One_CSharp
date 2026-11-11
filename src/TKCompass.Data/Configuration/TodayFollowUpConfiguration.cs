using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKCompass.Domain.Models;

namespace TKCompass.Data.Configuration;

public class TodayFollowUpConfiguration : IEntityTypeConfiguration<TodayFollowUpDto>
{
    public void Configure(EntityTypeBuilder<TodayFollowUpDto> builder)
    {
        builder.ToView("v_today_follow_up");
        builder.HasNoKey();

        // 属性映射（视图列名 → DTO 属性）
        builder.Property(e => e.CreatorId).HasColumnName("creator_id");
        builder.Property(e => e.Handle).HasColumnName("handle");
        builder.Property(e => e.Nickname).HasColumnName("nickname");
        builder.Property(e => e.PreferredName).HasColumnName("preferred_name");
        builder.Property(e => e.QualityLevel).HasColumnName("quality_level");
        builder.Property(e => e.ShipmentCount).HasColumnName("active_shipment_count");
        builder.Property(e => e.VideoCount).HasColumnName("pending_video_count");
        builder.Property(e => e.FollowUpStatus).HasColumnName("follow_up_status");
    }
}
