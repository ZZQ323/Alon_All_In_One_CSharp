using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKCompass.Domain.Models;

namespace TKCompass.Data.Configuration;

public class StatusFlowHistoryConfiguration : IEntityTypeConfiguration<StatusFlowHistory>
{
    public void Configure(EntityTypeBuilder<StatusFlowHistory> builder)
    {
        builder.ToTable("status_flow_history");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.FromStatus).HasMaxLength(50);
        builder.Property(e => e.ToStatus).HasMaxLength(50);
        builder.Property(e => e.ActionType).HasMaxLength(50);
        builder.Property(e => e.Notes).HasMaxLength(1000);
        
        // 索引
        builder.HasIndex(e => e.CreatorId);
        builder.HasIndex(e => e.ActionDate);
        
        // 关系
        builder.HasOne(e => e.Creator)
            .WithMany(e => e.StatusHistories)
            .HasForeignKey(e => e.CreatorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
