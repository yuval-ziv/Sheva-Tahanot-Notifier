using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Repositories.Abstract;

namespace ShevaTahanotNotifier.Database.Repositories;

public interface INotificationScheduleRepository : IGenericRepository<NotificationSchedule>
{
    IQueryable<NotificationSchedule> GetAllByUserId(Guid userId, bool tracking = false);
    IQueryable<NotificationSchedule> GetAllEnabledByUserId(Guid userId, bool tracking = false);
    IQueryable<NotificationSchedule> GetAllDisabledByUserId(Guid userId, bool tracking = false);
}