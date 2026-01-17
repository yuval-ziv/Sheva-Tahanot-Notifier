using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Entities.NotificationProviderConfiguration;
using ShevaTahanotNotifier.Exceptions;

namespace ShevaTahanotNotifier.Services.Notifiers;

public abstract class BaseNotifierService<T> : INotifierService  where T : BaseNotificationProviderConfiguration
{
    public abstract NotificationProvider NotificationProvider { get; }
    public abstract Task NotifyAsync(User user, CancellationToken cancellationToken = default);

    protected T GetConfiguration(User user)
    {
        if (user.Configuration is T configuration)
        {
            return configuration;
        }

        throw new InvalidConfigurationTypeException(typeof(T), user.Configuration?.GetType());
    }
    
}