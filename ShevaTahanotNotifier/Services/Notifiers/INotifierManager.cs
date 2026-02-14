namespace ShevaTahanotNotifier.Services.Notifiers;

public interface INotifierManager
{
    Task NotifyAsync(Guid notificationId, CancellationToken cancellationToken = default);
}