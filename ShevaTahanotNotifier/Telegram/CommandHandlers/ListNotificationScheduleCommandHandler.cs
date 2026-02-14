using Microsoft.EntityFrameworkCore;
using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Repositories;
using ShevaTahanotNotifier.Telegram.CommandHandlers.Abstraction;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = ShevaTahanotNotifier.Database.Entities.User;

namespace ShevaTahanotNotifier.Telegram.CommandHandlers;

public class ListNotificationScheduleCommandHandler : ICommandHandler
{
    private readonly ILogger<ListNotificationScheduleCommandHandler> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly ITelegramUserRepository _telegramUserRepository;

    public ListNotificationScheduleCommandHandler(ILogger<ListNotificationScheduleCommandHandler> logger, ITelegramBotClient bot, ITelegramUserRepository telegramUserRepository)
    {
        _logger = logger;
        _telegramUserRepository = telegramUserRepository;
        _bot = bot;
    }

    public string Command => "/list";
    public string Description => "lists all notification schedules";

    public async Task<Message> HandleCommandAsync(Message message, CancellationToken cancellationToken = default)
    {
        long chatId = message.Chat.Id;
        _logger.LogDebug("Handling list notification schedules command from {ChatId}", chatId);

        List<User> users = await _telegramUserRepository.GetAllByChatId(chatId).ToListAsync(cancellationToken);

        if (users.Count <= 0)
        {
            _logger.LogDebug("User is not registered with chat {ChatId}", chatId);
            return await _bot.SendMessage(chatId, "You are not registered", cancellationToken: cancellationToken);
        }

        if (!users.SelectMany(user => user.NotificationSchedules ?? Enumerable.Empty<NotificationSchedule>()).Any())
        {
            _logger.LogDebug("User has no notification schedules with chat {ChatId}", chatId);
            return await _bot.SendMessage(chatId, "You don't have any notification schedules.", cancellationToken: cancellationToken);
        }

        IEnumerable<string> notificationSchedules = users.SelectMany(user => user.NotificationSchedules ?? Enumerable.Empty<NotificationSchedule>())
            .OrderByDescending(schedule => schedule.Enabled)
            .ThenBy(schedule => schedule.Day)
            .ThenBy(schedule => schedule.Hour)
            .ThenBy(schedule => schedule.Minute)
            .Select(schedule => $"• {schedule.ButtonText}");

        var text = $"Your notification schedules:{Environment.NewLine}{string.Join(Environment.NewLine, notificationSchedules)}";

        return await _bot.SendMessage(chatId, text, cancellationToken: cancellationToken);
    }
}