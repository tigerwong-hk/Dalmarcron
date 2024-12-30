using Audit.EntityFramework;
using Dalmarcron.Scheduler.Core.Constants;
using Dalmarcron.Scheduler.EntityFrameworkCore.Entities;
using Dalmarkit.Common.AuditTrail;
using Dalmarkit.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dalmarcron.Scheduler.EntityFrameworkCore.Contexts;

public class DalmarcronSchedulerDbContext(DbContextOptions options) : AuditDbContext(options)
{
    private static readonly EnumToStringConverter<ApiMethod> ApiMethodConverter = new();
    private static readonly EnumToStringConverter<ApiType> ApiTypeConverter = new();
    private static readonly EnumToStringConverter<PublicationState> PublicationStateConverter = new();

    public DbSet<ApiLog> ApiLogs { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    public DbSet<JobPublishedTransaction> JobPublishedTransactions { get; set; } = null!;
    public DbSet<JobUnpublishedTransaction> JobUnpublishedTransactions { get; set; } = null!;
    public DbSet<ScheduledJob> ScheduledJobs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.BuildApiLogEntity();

        _ = modelBuilder.BuildAuditLogEntity();

        _ = modelBuilder.BuildReadWriteEntity<ScheduledJob>();
        _ = modelBuilder.Entity<ScheduledJob>()
            .Property(e => e.ScheduledJobId)
            .HasDefaultValueSql(ModelBuilderExtensions.DefaultGuidValueSql);
        _ = modelBuilder.Entity<ScheduledJob>()
            .Property(e => e.ApiMethod)
            .HasConversion(ApiMethodConverter)
            .HasMaxLength(20);
        _ = modelBuilder.Entity<ScheduledJob>()
            .Property(e => e.ApiType)
            .HasConversion(ApiTypeConverter)
            .HasMaxLength(20);
        _ = modelBuilder.Entity<ScheduledJob>()
            .Property(e => e.PublicationState)
            .HasConversion(PublicationStateConverter)
            .HasMaxLength(20);
        _ = modelBuilder.Entity<ScheduledJob>()
            .HasIndex(e => e.JobName)
            .HasFilter(@"""IsDeleted"" = false")
            .IsUnique();

        _ = modelBuilder.BuildReadOnlyEntity<JobPublishedTransaction>();
        _ = modelBuilder.Entity<JobPublishedTransaction>()
            .Property(e => e.JobPublishedTransactionId)
            .HasDefaultValueSql(ModelBuilderExtensions.DefaultGuidValueSql);
        _ = modelBuilder.Entity<JobPublishedTransaction>()
            .Property(e => e.ApiMethod)
            .HasConversion(ApiMethodConverter)
            .HasMaxLength(20);
        _ = modelBuilder.Entity<JobPublishedTransaction>()
            .Property(e => e.ApiType)
            .HasConversion(ApiTypeConverter)
            .HasMaxLength(20);

        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.BuildReadOnlyEntity<JobUnpublishedTransaction>();
        _ = modelBuilder.Entity<JobUnpublishedTransaction>()
            .Property(e => e.JobUnpublishedTransactionId)
            .HasDefaultValueSql(ModelBuilderExtensions.DefaultGuidValueSql);
        _ = modelBuilder.Entity<JobUnpublishedTransaction>()
            .Property(e => e.ApiMethod)
            .HasConversion(ApiMethodConverter)
            .HasMaxLength(20);
        _ = modelBuilder.Entity<JobUnpublishedTransaction>()
            .Property(e => e.ApiType)
            .HasConversion(ApiTypeConverter)
            .HasMaxLength(20);

        base.OnModelCreating(modelBuilder);
    }
}
