using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Repositories.Abstract;

namespace ShevaTahanotNotifier.Database.Repositories;

public class NotificationScheduleRepository : GenericRepository<NotificationSchedule>, INotificationScheduleRepository
{
    private const bool Enabled = true;
    private const bool Disabled = false;

    public NotificationScheduleRepository(NotifierContext context) : base(context)
    {
    }

    public IQueryable<NotificationSchedule> GetAllByUserId(Guid userId, bool tracking = false)
    {
        return GetAll(tracking).Where(notificationSchedule => notificationSchedule.UserId == userId);
    }

    public IQueryable<NotificationSchedule> GetAllEnabledByUserId(Guid userId, bool tracking = false)
    {
        return GetAllByUserIdAndEnabled(userId, Enabled, tracking);
    }

    public IQueryable<NotificationSchedule> GetAllDisabledByUserId(Guid userId, bool tracking = false)
    {
        return GetAllByUserIdAndEnabled(userId, Disabled, tracking);
    }

    private IQueryable<NotificationSchedule> GetAllByUserIdAndEnabled(Guid userId, bool enabled, bool tracking)
    {
        return GetAllByUserId(userId, tracking).Where(notificationSchedule => enabled);
    }
}