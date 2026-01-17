namespace ShevaTahanotNotifier.Database.Entities.NotificationProviderConfiguration;

public abstract class BaseNotificationProviderConfiguration : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
}