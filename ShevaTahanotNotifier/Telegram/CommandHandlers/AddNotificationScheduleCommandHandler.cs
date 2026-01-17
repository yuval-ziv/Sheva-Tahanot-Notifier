using Microsoft.EntityFrameworkCore;
using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = ShevaTahanotNotifier.Database.Entities.User;

namespace ShevaTahanotNotifier.Telegram.CommandHandlers;

public class AddNotificationScheduleCommandHandler : ICommandHandler
{
    public string Command => "/add";
    public string Description => "adds a new notification schedule";

    public Task<Message> HandleCommandAsync(Message message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class RemoveNotificationScheduleCommandHandler : ICommandHandler
{
    private readonly ILogger<RemoveNotificationScheduleCommandHandler> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly ITelegramUserRepository _telegramUserRepository;
    private readonly INotificationScheduleRepository _notificationScheduleRepository;

    public RemoveNotificationScheduleCommandHandler(ILogger<RemoveNotificationScheduleCommandHandler> logger, ITelegramBotClient bot, ITelegramUserRepository telegramUserRepository,
        INotificationScheduleRepository notificationScheduleRepository)
    {
        _logger = logger;
        _telegramUserRepository = telegramUserRepository;
        _notificationScheduleRepository = notificationScheduleRepository;
        _bot = bot;
    }

    public string Command => "/remove";
    public string Description => "removes a notification schedule";

    public async Task<Message> HandleCommandAsync(Message message, CancellationToken cancellationToken = default)
    {
        long chatId = message.Chat.Id;
        _logger.LogDebug("Handling add notification schedule command from {ChatId}", chatId);

        List<User> users = await _telegramUserRepository.GetAllByChatId(chatId).ToListAsync(cancellationToken);

        if (users.Count <= 0)
        {
            _logger.LogDebug("User is not registered with chat {ChatId}", chatId);
            return await _bot.SendMessage(chatId, $"User {message.From?.Username} is not registered", cancellationToken: cancellationToken);
        }

        users.SelectMany(user => user.NotificationSchedules).Select(schedule => ToButton(chatId, schedule));

        InlineKeyboardMarkup keyboard = new()
        {
            InlineKeyboard =
            [
                [
                    new InlineKeyboardButton
                    {
                        Text = "Sunday",
                        CallbackData = "Sunday",
                    }
                ]
            ],
        };
        return await _bot.SendMessage(chatId, $"Choose a notification to remove", replyMarkup: keyboard, cancellationToken: cancellationToken);
    }

    private InlineKeyboardButton ToButton(long chatId, NotificationSchedule notificationSchedule)
    {
        return new InlineKeyboardButton
        {
            Text = $"{notificationSchedule.DayOfWeek} - {notificationSchedule.Hour}:{notificationSchedule.Minute}",
            CallbackData = $"delete_{chatId}_{notificationSchedule.Id}",
        };
    }
}

public class EnableNotificationScheduleCommandHandler : ICommandHandler
{
    public string Command => "/enable";
    public string Description => "enables a notification schedule";

    public Task<Message> HandleCommandAsync(Message message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class DisableNotificationScheduleCommandHandler : ICommandHandler
{
    public string Command => "/disable";
    public string Description => "disables a notification schedule";

    public Task<Message> HandleCommandAsync(Message message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}