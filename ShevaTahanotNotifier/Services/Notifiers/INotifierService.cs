using ShevaTahanotNotifier.Database.Entities;

namespace ShevaTahanotNotifier.Services.Notifiers;

public interface INotifierService
{
    NotificationProvider NotificationProvider { get; }
    Task NotifyAsync(User user, CancellationToken cancellationToken = default);
}