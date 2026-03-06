using Microsoft.EntityFrameworkCore;
using ShevaTahanotNotifier.Coravel;
using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Repositories;

namespace ShevaTahanotNotifier.Services;

public class NotificationScheduleService : INotificationScheduleService
{
    private readonly INotificationScheduleRepository _notificationScheduleRepository;
    private readonly ICoravelService _coravelService;

    public NotificationScheduleService(INotificationScheduleRepository notificationScheduleRepository, ICoravelService coravelService)
    {
        _notificationScheduleRepository = notificationScheduleRepository;
        _coravelService = coravelService;
    }

    public async Task<NotificationSchedule> CreateAsync(NotificationSchedule notificationSchedule, CancellationToken cancellationToken)
    {
        NotificationSchedule savedSchedule = await _notificationScheduleRepository.CreateAsync(notificationSchedule, saveChanges: true, cancellationToken);
        _coravelService.Register(notificationSchedule);
        return savedSchedule;
    }

    public async Task DeleteAsync(Guid notificationScheduleId, CancellationToken cancellationToken)
    {
        await _notificationScheduleRepository.DeleteAsync(notificationScheduleId, saveChanges: true, cancellationToken);
        _coravelService.Deregister(notificationScheduleId);
    }

    public async Task EnableAsync(Guid notificationScheduleId, CancellationToken cancellationToken)
    {
        NotificationSchedule? notification = await _notificationScheduleRepository.GetByIdAsync(notificationScheduleId, tracking: true, cancellationToken);
        if (notification is not null)
        {
            notification.Enabled = true;
            await _notificationScheduleRepository.UpdateAsync(notification, saveChanges: true, cancellationToken);
            _coravelService.Register(notification);
        }
    }

    public async Task DisableAsync(Guid notificationScheduleId, CancellationToken cancellationToken)
    {
        NotificationSchedule? notification = await _notificationScheduleRepository.GetByIdAsync(notificationScheduleId, tracking: true, cancellationToken);
        if (notification is not null)
        {
            notification.Enabled = false;
            await _notificationScheduleRepository.UpdateAsync(notification, saveChanges: true, cancellationToken);
        }

        _coravelService.Deregister(notificationScheduleId);
    }

    public async Task EnableAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        List<NotificationSchedule> notifications = await _notificationScheduleRepository.GetAll(tracking: true).Where(notification => notification.UserId == userId).ToListAsync(cancellationToken: cancellationToken);
        if (notifications.Count == 0)
        {
            return;
        }
        notifications.ForEach(notification =>
        {
            notification.Enabled = true;
            _coravelService.Deregister(notification.Id);
        });
        
        await _notificationScheduleRepository.UpdateAsync(notifications, cancellationToken: cancellationToken);
    }

    public async Task DisableAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        List<NotificationSchedule> notifications = await _notificationScheduleRepository.GetAll(tracking: true).Where(notification => notification.UserId == userId)
            .ToListAsync(cancellationToken: cancellationToken);
        if (notifications.Count == 0)
        {
            return;
        }

        notifications.ForEach(notification =>
        {
            notification.Enabled = false;
            _coravelService.Register(notification);
        });

        await _notificationScheduleRepository.UpdateAsync(notifications, cancellationToken: cancellationToken);
    }
}