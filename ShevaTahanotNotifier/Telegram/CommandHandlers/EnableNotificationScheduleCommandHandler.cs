using Microsoft.EntityFrameworkCore;
using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Repositories;
using ShevaTahanotNotifier.Telegram.CallbackHandlers;
using ShevaTahanotNotifier.Telegram.CommandHandlers.Abstraction;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = ShevaTahanotNotifier.Database.Entities.User;

namespace ShevaTahanotNotifier.Telegram.CommandHandlers;

public class EnableNotificationScheduleCommandHandler : ICommandHandler
{
    private readonly ILogger<EnableNotificationScheduleCommandHandler> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly ITelegramUserRepository _telegramUserRepository;

    public EnableNotificationScheduleCommandHandler(ILogger<EnableNotificationScheduleCommandHandler> logger, ITelegramBotClient bot, ITelegramUserRepository telegramUserRepository)
    {
        _logger = logger;
        _telegramUserRepository = telegramUserRepository;
        _bot = bot;
    }

    public string Command => "/enable";
    public string Description => "enables a notification schedule";

    public async Task<Message> HandleCommandAsync(Message message, CancellationToken cancellationToken = default)
    {
        long chatId = message.Chat.Id;
        _logger.LogDebug("Handling enable notification schedule command from {ChatId}", chatId);

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

        if (users.SelectMany(user => user.NotificationSchedules ?? Enumerable.Empty<NotificationSchedule>()).All(schedule => schedule.Enabled))
        {
            _logger.LogDebug("User has no disabled notification schedules with chat {ChatId}", chatId);
            return await _bot.SendMessage(chatId, "You don't have any disabled notification schedules.", cancellationToken: cancellationToken);
        }

        IEnumerable<InlineKeyboardButton> buttons = users.SelectMany(user => user.NotificationSchedules ?? Enumerable.Empty<NotificationSchedule>())
            .Where(schedule => !schedule.Enabled)
            .Select(schedule => ToButton(chatId, schedule));

        InlineKeyboardMarkup keyboard = new()
        {
            InlineKeyboard = buttons.Chunk(2).Prepend(GetAllButton(chatId)).Prepend(GetCancelButton(chatId)),
        };
        return await _bot.SendMessage(chatId, "Choose a notification to enable", replyMarkup: keyboard, cancellationToken: cancellationToken);
    }

    private InlineKeyboardButton ToButton(long chatId, NotificationSchedule notificationSchedule)
    {
        return new InlineKeyboardButton
        {
            Text = notificationSchedule.ButtonText,
            CallbackData = $"{EnableNotificationCallbackHandler.CallbackName}_{chatId}_{notificationSchedule.Id}",
        };
    }

    private InlineKeyboardButton[] GetCancelButton(long chatId)
    {
        return
        [
            new InlineKeyboardButton
            {
                Text = "Cancel",
                CallbackData = $"{EnableNotificationCallbackHandler.CallbackName}_{chatId}_cancel",
            },
        ];
    }

    private InlineKeyboardButton[] GetAllButton(long chatId)
    {
        return
        [
            new InlineKeyboardButton
            {
                Text = "All",
                CallbackData = $"{EnableNotificationCallbackHandler.CallbackName}_{chatId}_all",
            },
        ];
    }
}