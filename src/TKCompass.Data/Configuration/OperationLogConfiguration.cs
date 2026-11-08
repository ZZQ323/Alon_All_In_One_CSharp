using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKCompass.Domain.Models;

namespace TKCompass.Data.Configuration;

public class OperationLogConfiguration : IEntityTypeConfiguration<OperationLog>
{
    public void Configure(EntityTypeBuilder<OperationLog> builder)
    {
        builder.ToTable("operation_log");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.OperationType).IsRequired().HasMaxLength(50);
        builder.Property(e => e.EntityType).IsRequired().HasMaxLength(50);
        builder.Property(e => e.EntityId).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.SessionId).HasMaxLength(50);
        
        // 大文本字段
        builder.Property(e => e.BeforeJson);
        builder.Property(e => e.AfterJson);
        
        // 索引
        builder.HasIndex(e => e.OperationType);
        builder.HasIndex(e => e.EntityType);
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.SessionId);
        builder.HasIndex(e => e.Undone);
    }
}
