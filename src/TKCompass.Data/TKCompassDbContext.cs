using Microsoft.EntityFrameworkCore;
using TKCompass.Domain.Models;

namespace TKCompass.Data;

public class TKCompassDbContext : DbContext
{
    public DbSet<Creator> Creators { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<CreatorContact> CreatorContacts { get; set; }
    public DbSet<StatusFlowHistory> StatusFlowHistories { get; set; }
    public DbSet<OperationLog> OperationLogs { get; set; }
    public DbSet<Dictionary> Dictionaries { get; set; }
    public DbSet<CreatorType> CreatorTypes { get; set; }
    public DbSet<ProductCode> ProductCodes { get; set; }
    public DbSet<TagDefinition> TagDefinitions { get; set; }
    public DbSet<CreatorTag> CreatorTags { get; set; }
    public DbSet<Relationship> Relationships { get; set; }
    public DbSet<BatchRemindLog> BatchRemindLogs { get; set; }
    public DbSet<CreatorDashboardDto> CreatorDashboards { get; set; } = null!;
    public DbSet<TodayFollowUpDto> TodayFollowUps { get; set; } = null!;
    public DbSet<CreatorFulfillmentDto> CreatorFulfillments { get; set; } = null!;

    public TKCompassDbContext(DbContextOptions<TKCompassDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 应用所有配置
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TKCompassDbContext).Assembly);
    }
}
