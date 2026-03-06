using ShevaTahanotNotifier.Database.Entities;

namespace ShevaTahanotNotifier.Services;

public interface INotificationScheduleService
{
    Task<NotificationSchedule> CreateAsync(NotificationSchedule notificationSchedule, CancellationToken cancellationToken);
    Task DeleteAsync(Guid notificationScheduleId, CancellationToken cancellationToken);
    Task EnableAsync(Guid notificationScheduleId, CancellationToken cancellationToken);
    Task DisableAsync(Guid notificationScheduleId, CancellationToken cancellationToken);
    Task EnableAllByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task DisableAllByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}