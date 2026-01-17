using Microsoft.EntityFrameworkCore;
using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = ShevaTahanotNotifier.Database.Entities.User;

namespace ShevaTahanotNotifier.Telegram.CommandHandlers;

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
        _logger.LogDebug("Handling remove notification schedule command from {ChatId}", chatId);

        List<User> users = await _telegramUserRepository.GetAllByChatId(chatId).ToListAsync(cancellationToken);

        if (users.Count <= 0)
        {
            _logger.LogDebug("User is not registered with chat {ChatId}", chatId);
            return await _bot.SendMessage(chatId, $"User {message.From?.Username} is not registered", cancellationToken: cancellationToken);
        }

        IEnumerable<InlineKeyboardButton> buttons = users.SelectMany(user => user.NotificationSchedules ?? Enumerable.Empty<NotificationSchedule>()).Select(schedule => ToButton(chatId, schedule));

        InlineKeyboardMarkup keyboard = new()
        {
            InlineKeyboard =
            [
                buttons,
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