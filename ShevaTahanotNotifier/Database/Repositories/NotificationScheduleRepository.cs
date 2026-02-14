using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Repositories.Abstract;

namespace ShevaTahanotNotifier.Database.Repositories;

public class NotificationScheduleRepository : GenericRepository<NotificationSchedule>, INotificationScheduleRepository
{
    public NotificationScheduleRepository(NotifierContext context) : base(context)
    {
    }
}