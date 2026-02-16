using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Event;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.EntityFrameworkCore;
using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Repositories;
using ShevaTahanotNotifier.ExtensionMethods;

namespace ShevaTahanotNotifier.Coravel;

public class CoravelService : ICoravelService
{
    private readonly ILogger<CoravelService> _logger;
    private readonly Scheduler _scheduler;
    private readonly IServiceProvider _services;
    private readonly INotificationScheduleRepository _notificationScheduleRepository;
    private TimeZoneInfo _israelTimeZone;

    public CoravelService(ILogger<CoravelService> logger, IScheduler scheduler, IServiceProvider services, INotificationScheduleRepository notificationScheduleRepository)
    {
        if (scheduler is not Scheduler baseScheduler)
        {
            throw new ArgumentException($"Scheduler is not supported with type {scheduler.GetType().FullName}", nameof(scheduler));
        }

        _logger = logger;
        _scheduler = baseScheduler;
        _services = services;
        _notificationScheduleRepository = notificationScheduleRepository;
        _israelTimeZone = GetIsraelTimeZoneOrLocal();
    }

    private TimeZoneInfo GetIsraelTimeZoneOrLocal()
    {
        if (TimeZoneInfo.TryFindSystemTimeZoneById("Asia/Jerusalem", out var israelTimeZone))
        {
            return israelTimeZone;
        }
        else
        {
            _logger.LogError("Failed to get Israel's time zone, falling back to local with utc offset {UtcOffset}", _israelTimeZone.BaseUtcOffset);
            return TimeZoneInfo.Local;
        }
    }

    public void Register(NotificationSchedule schedule)
    {
        var cron = $"{schedule.Minute} {schedule.Hour} * * {schedule.Day.ToCronDayOfWeek()}";
        _logger.LogDebug("Registering coravel schedule for notification schedule {NotificationScheduleId} with cron {Cron} for user {UserId}", schedule.Id, cron, schedule.UserId);

        IScheduledEventConfiguration eventConfiguration = _scheduler.ScheduleWithParams<NotifierInvocable>(_services.CreateAsyncScope(), schedule.Id)
            .Cron(cron).Zoned(_israelTimeZone);
        (eventConfiguration as ScheduledEvent)?.AssignUniqueIndentifier(schedule.Id.ToString());
        _logger.LogDebug("Registered coravel schedule for notification schedule {NotificationScheduleId} with cron {Cron} for user {UserId}", schedule.Id, cron, schedule.UserId);
    }

    public void Deregister(Guid id)
    {
        _logger.LogDebug("Deregister coravel schedule for notification schedule {NotificationScheduleId} ", id);
        _scheduler.TryUnschedule(id.ToString());
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _notificationScheduleRepository.GetAll().Where(notificationSchedule => notificationSchedule.Enabled).AsAsyncEnumerable().ForEachAsync(Register, cancellationToken: cancellationToken);
    }
}