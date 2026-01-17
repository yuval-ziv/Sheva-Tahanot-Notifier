using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShevaTahanotNotifier.Database.Entities.NotificationProviderConfiguration;

namespace ShevaTahanotNotifier.Database.Entities;

public class User : BaseEntity
{
    public required NotificationProvider Provider { get; set; }
    public virtual BaseNotificationProviderConfiguration? Configuration { get; set; }
    public virtual List<NotificationSchedule>? NotificationSchedules { get; set; }
}