using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKCompass.Domain.Models;

namespace TKCompass.Data.Configuration;

public class RelationshipConfiguration : IEntityTypeConfiguration<Relationship>
{
    public void Configure(EntityTypeBuilder<Relationship> builder)
    {
        builder.ToTable("relationships");
        
        // 主键是 CreatorId（一对一关系，每个达人一条关系记录）
        builder.HasKey(e => e.CreatorId);
        
        builder.Property(e => e.PreferredName).HasMaxLength(100);
        builder.Property(e => e.RelationshipNotes).HasMaxLength(2000);
        builder.Property(e => e.VideoStyle).HasMaxLength(200);
        builder.Property(e => e.NextAction).HasMaxLength(500);
        builder.Property(e => e.CurrentCommission).HasMaxLength(100);
        
        // 关系：一对一（Relationship 与 Creator 共享主键）
        builder.HasOne(e => e.Creator)
            .WithOne()
            .HasForeignKey<Relationship>(e => e.CreatorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
