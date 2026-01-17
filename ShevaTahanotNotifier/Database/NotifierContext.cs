using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShevaTahanotNotifier.Configuration;
using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Entities.NotificationProviderConfiguration;
using Telegram.Bot.Types;
using User = ShevaTahanotNotifier.Database.Entities.User;

namespace ShevaTahanotNotifier.Database;

public class NotifierContext : DbContext
{
    private readonly NotifierContextOptions _configuration;
    public DbSet<User> Users { get; set; }
    public DbSet<NotificationSchedule> NotificationSchedules { get; set; }

    public NotifierContext(DbContextOptions<NotifierContext> options, IOptionsMonitor<NotifierContextOptions> optionsMonitor) : base(options)
    {
        _configuration = optionsMonitor.CurrentValue;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        switch (_configuration.DatabaseProviderType)
        {
            case DatabaseProviderType.Sqlite:
                optionsBuilder.UseSqlite(_configuration.ConnectionString);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_configuration.DatabaseProviderType), _configuration.DatabaseProviderType, null);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().Navigation(u => u.Configuration).AutoInclude();
        modelBuilder.Entity<User>().Navigation(u => u.NotificationSchedules).AutoInclude();

        modelBuilder.Entity<User>()
            .HasMany(user => user.NotificationSchedules)
            .WithOne(notifySchedule => notifySchedule.User)
            .HasForeignKey(notifySchedule => notifySchedule.UserId);

        modelBuilder.Entity<User>()
            .HasOne(user => user.Configuration)
            .WithOne(configuration => configuration.User)
            .HasForeignKey<BaseNotificationProviderConfiguration>(configuration => configuration.UserId);

        modelBuilder.Entity<BaseNotificationProviderConfiguration>().UseTpcMappingStrategy();
        modelBuilder.Entity<TelegramNotificationProviderConfiguration>().ToTable(nameof(TelegramNotificationProviderConfiguration));
        modelBuilder.Entity<EmailNotificationProviderConfiguration>().ToTable(nameof(EmailNotificationProviderConfiguration));
    }
}