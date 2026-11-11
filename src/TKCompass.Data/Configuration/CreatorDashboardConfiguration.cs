using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKCompass.Domain.Models;

namespace TKCompass.Data.Configuration;

public class CreatorDashboardConfiguration : IEntityTypeConfiguration<CreatorDashboardDto>
{
    public void Configure(EntityTypeBuilder<CreatorDashboardDto> builder)
    {
        builder.ToView("v_creator_dashboard");
        builder.HasNoKey();

        // 属性映射（视图列名 → DTO 属性）
        builder.Property(e => e.CreatorId).HasColumnName("creator_id");
        builder.Property(e => e.Handle).HasColumnName("handle");
        builder.Property(e => e.Nickname).HasColumnName("nickname");
        builder.Property(e => e.PreferredName).HasColumnName("preferred_name");
        builder.Property(e => e.QualityLevel).HasColumnName("quality_level");
        builder.Property(e => e.MissingVideoCount).HasColumnName("missing_video_count");
        builder.Property(e => e.MissingCodeCount).HasColumnName("missing_code_count");
        builder.Property(e => e.ContinuousCount).HasColumnName("continuous_count");
        builder.Property(e => e.TotalGmv).HasColumnName("total_gmv");
        builder.Property(e => e.LastContactDate).HasColumnName("latest_contact_date");
        builder.Property(e => e.TodayFollowUp).HasColumnName("today_follow_up");
        builder.Property(e => e.ShipmentCount).HasColumnName("shipment_count");
        builder.Property(e => e.VideoCount).HasColumnName("video_count");
        builder.Property(e => e.ValidVideoCount).HasColumnName("valid_video_count");
    }
}
