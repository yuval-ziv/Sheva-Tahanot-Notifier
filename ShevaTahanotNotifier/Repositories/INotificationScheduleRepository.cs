using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Repositories.Abstract;

namespace ShevaTahanotNotifier.Repositories;

public interface INotificationScheduleRepository : IGenericRepository<NotificationSchedule>
{
    IQueryable<NotificationSchedule> GetAllByUserId(Guid userId, bool tracking = false);
    IQueryable<NotificationSchedule> GetAllEnabledByUserId(Guid userId, bool tracking = false);
    IQueryable<NotificationSchedule> GetAllDisabledByUserId(Guid userId, bool tracking = false);
}