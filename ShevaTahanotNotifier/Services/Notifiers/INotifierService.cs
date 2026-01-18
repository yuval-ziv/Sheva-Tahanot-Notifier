using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Entities.Enums;

namespace ShevaTahanotNotifier.Services.Notifiers;

public interface INotifierService
{
    NotificationProvider NotificationProvider { get; }
    Task NotifyAsync(User user, CancellationToken cancellationToken = default);
}