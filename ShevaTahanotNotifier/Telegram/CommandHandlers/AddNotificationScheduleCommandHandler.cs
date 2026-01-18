using Microsoft.EntityFrameworkCore;
using ShevaTahanotNotifier.Database.Entities.Enums;
using ShevaTahanotNotifier.Database.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = ShevaTahanotNotifier.Database.Entities.User;

namespace ShevaTahanotNotifier.Telegram.CommandHandlers;

public class AddNotificationScheduleCommandHandler : ICommandHandler
{
    private readonly ILogger<AddNotificationScheduleCommandHandler> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly ITelegramUserRepository _telegramUserRepository;

    public AddNotificationScheduleCommandHandler(ILogger<AddNotificationScheduleCommandHandler> logger, ITelegramBotClient bot, ITelegramUserRepository telegramUserRepository)
    {
        _logger = logger;
        _bot = bot;
        _telegramUserRepository = telegramUserRepository;
    }

    public string Command => "/add";
    public string Description => "adds a new notification schedule";

    public async Task<Message> HandleCommandAsync(Message message, CancellationToken cancellationToken = default)
    {
        long chatId = message.Chat.Id;
        _logger.LogDebug("Handling add notification schedule command from {ChatId}", chatId);

        List<User> users = await _telegramUserRepository.GetAllByChatId(chatId).ToListAsync(cancellationToken);

        if (users.Count <= 0)
        {
            _logger.LogDebug("User is not registered with chat {ChatId}", chatId);
            return await _bot.SendMessage(chatId, $"You are not registered! Please register and try again.", cancellationToken: cancellationToken);
        }

        IEnumerable<InlineKeyboardButton> buttons = Day.GetValues().Select(day => ToButton(chatId, day));

        InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup()
        {
            InlineKeyboard = buttons.Chunk(2),
        };
        return await _bot.SendMessage(chatId, "In which day(s) do you want to add a notification?", replyMarkup: keyboard, cancellationToken: cancellationToken);
    }

    private InlineKeyboardButton ToButton(long chatId, Day day)
    {
        string dayString = day.ToStringFast();
        return InlineKeyboardButton.WithCallbackData(dayString, $"add_{chatId}_{dayString}");
    }
}