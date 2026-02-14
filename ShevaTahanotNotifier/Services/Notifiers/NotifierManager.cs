using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Entities.Enums;
using ShevaTahanotNotifier.Database.Repositories;

namespace ShevaTahanotNotifier.Services.Notifiers;

public class NotifierManager : INotifierManager
{
    private readonly ILogger<NotifierManager> _logger;
    private readonly INotificationScheduleRepository _notificationScheduleRepository;
    private readonly Dictionary<NotificationProvider, INotificationProviderService> _notificationProviderToService;

    public NotifierManager(ILogger<NotifierManager> logger, INotificationScheduleRepository notificationScheduleRepository, IEnumerable<INotificationProviderService> notificationProviderServices)
    {
        _logger = logger;
        _notificationScheduleRepository = notificationScheduleRepository;
        _notificationProviderToService = notificationProviderServices.ToDictionary(notificationProvider => notificationProvider.NotificationProvider, notificationProvider => notificationProvider);
    }

    public async Task NotifyAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        NotificationSchedule? notificationSchedule = await _notificationScheduleRepository.GetByIdAsync(notificationId, tracking: true, cancellationToken: cancellationToken);
        if (notificationSchedule is null || !notificationSchedule.Enabled || notificationSchedule.User is null)
        {
            _logger.LogDebug("Notification is disabled or deleted, removing job");
            return;
        }

        if (!_notificationProviderToService.TryGetValue(notificationSchedule.User.Provider, out INotificationProviderService? provider))
        {
            _logger.LogError("Unable to  find notification provider {Provider} for notification {NotificationId} and user {UserId}", notificationSchedule.User.Provider, notificationId,
                notificationSchedule.UserId);
            return;
        }

        await provider.NotifyAsync(notificationSchedule.User, cancellationToken);
    }
}