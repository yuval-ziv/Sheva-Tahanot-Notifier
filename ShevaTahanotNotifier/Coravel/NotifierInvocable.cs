using Coravel.Invocable;
using ShevaTahanotNotifier.Services.Notifiers;

namespace ShevaTahanotNotifier.Coravel;

public class NotifierInvocable : IInvocable
{
    private readonly AsyncServiceScope _serviceScope;
    private readonly Guid _notificationId;

    public NotifierInvocable(AsyncServiceScope serviceScope, Guid notificationId)
    {
        _serviceScope = serviceScope;
        _notificationId = notificationId;
    }

    public async Task Invoke()
    {
        var logger = _serviceScope.ServiceProvider.GetRequiredService<ILogger<NotifierInvocable>>();
        var notifier = _serviceScope.ServiceProvider.GetRequiredService<INotifierManager>();

        logger.LogDebug("Invoking notifier for Notification {NotificationId}", _notificationId);
        await notifier.NotifyAsync(_notificationId);
        logger.LogDebug("Finished invoking notifier for Notification {NotificationId}", _notificationId);
    }
}