using ShevaTahanotNotifier.Database.Entities.Enums;
using ShevaTahanotNotifier.Database.Entities.NotificationProviderConfiguration;

namespace ShevaTahanotNotifier.Database.Entities;

public class User : BaseEntity
{
    public required NotificationProvider Provider { get; set; }
    public virtual BaseNotificationProviderConfiguration? Configuration { get; set; }
    public virtual List<NotificationSchedule>? NotificationSchedules { get; set; }
}