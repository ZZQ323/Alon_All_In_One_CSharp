using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKCompass.Domain.Models;

namespace TKCompass.Data.Configuration;

public class CreatorContactConfiguration : IEntityTypeConfiguration<CreatorContact>
{
    public void Configure(EntityTypeBuilder<CreatorContact> builder)
    {
        builder.ToTable("creator_contacts");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.ContactType).HasMaxLength(50);
        builder.Property(e => e.ContactValue).IsRequired().HasMaxLength(255);
        
        // 索引
        builder.HasIndex(e => e.CreatorId);
        builder.HasIndex(e => e.ContactType);
        
        // 关系
        builder.HasOne(e => e.Creator)
            .WithMany(e => e.Contacts)
            .HasForeignKey(e => e.CreatorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
