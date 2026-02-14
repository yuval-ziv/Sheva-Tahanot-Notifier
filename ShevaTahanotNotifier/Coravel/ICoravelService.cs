using ShevaTahanotNotifier.Database.Entities;

namespace ShevaTahanotNotifier.Coravel;

public interface ICoravelService
{
    void Register(NotificationSchedule schedule);
    void Deregister(Guid id);
    Task InitializeAsync(CancellationToken cancellationToken = default);
}