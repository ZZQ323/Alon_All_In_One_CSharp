using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKCompass.Domain.Models;

namespace TKCompass.Data.Configuration;

public class DictionaryConfiguration : IEntityTypeConfiguration<Dictionary>
{
    public void Configure(EntityTypeBuilder<Dictionary> builder)
    {
        builder.ToTable("dictionaries");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Type).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Code).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Label).IsRequired().HasMaxLength(100);
        
        // 索引
        builder.HasIndex(e => new { e.Type, e.Code }).IsUnique();
        builder.HasIndex(e => e.Type);
        builder.HasIndex(e => e.Deleted);
    }
}
