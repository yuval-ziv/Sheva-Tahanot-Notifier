using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Entities.Enums;
using ShevaTahanotNotifier.Database.Repositories;
using ShevaTahanotNotifier.Exceptions;
using ShevaTahanotNotifier.Telegram.ConversationHandlers;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = ShevaTahanotNotifier.Database.Entities.User;

namespace ShevaTahanotNotifier.Telegram.CallbackHandlers;

public class AddNotificationDayCallbackHandler : ICallbackHandler
{
    public const string CallbackName = "aad"; //after add day
    public const string SelectedDayExtraDataKey = "selected_day";

    private readonly ILogger<AddNotificationDayCallbackHandler> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly ITelegramUserRepository _telegramUserRepository;

    public AddNotificationDayCallbackHandler(ILogger<AddNotificationDayCallbackHandler> logger, ITelegramBotClient bot, ITelegramUserRepository telegramUserRepository)
    {
        _logger = logger;
        _bot = bot;
        _telegramUserRepository = telegramUserRepository;
    }

    public string CallbackPrefix => CallbackName;

    public async Task<(Message Message, Conversation? Conversation)> HandleCallbackAsync(CallbackQuery callback, CancellationToken cancellationToken = default)
    {
        Message? message = callback.Message;
        ArgumentNullException.ThrowIfNull(message);

        long chatId = message.Chat.Id;
        int messageId = message.MessageId;

        await _bot.EditMessageReplyMarkup(chatId: chatId, messageId: messageId, replyMarkup: null, cancellationToken: cancellationToken);

        _logger.LogDebug("Handling add day callback from {ChatId}", chatId);

        User? user = await _telegramUserRepository.GetByChatIdAsync(chatId, tracking: false, cancellationToken);

        if (user is null)
        {
            _logger.LogDebug("User is not registered with chat {ChatId}", chatId);
            return (await _bot.SendMessage(chatId, "You are not registered! Please register and try again.", cancellationToken: cancellationToken), null);
        }

        (long callbackDataChatId, Day day) = ParseCallbackData(callback.Data);

        if (callbackDataChatId != chatId)
        {
            throw new InvalidCallbackChatId(chatId, callbackDataChatId);
        }

        _logger.LogDebug("User from chat id {ChatId} wants to create a notification on day {Day}", chatId, day);
        Message messageSent = await _bot.EditMessageText(chatId: chatId, messageId: messageId, text: $"You chose {day.ToStringFast()}. At what time? Use 24-hour format (e.g. 17:30).",
            cancellationToken: cancellationToken);

        var conversation = new Conversation
        {
            UserId = user.Id,
            ChatId = chatId,
            CurrentStep = CallbackName,
            NextStep = AddHourConversationHandler.AddHourStepName,
            ExtraData = new Dictionary<string, string>
            {
                { SelectedDayExtraDataKey, day.ToStringFast() },
            },
        };

        return (messageSent, conversation);
    }

    private (long callbackDataChatId, Day day) ParseCallbackData(string? callbackData)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(callbackData);
        string callbackActualData = callbackData.Replace(CallbackPrefix + "_", string.Empty);
        string[] parts = callbackActualData.Split("_");
        if (parts.Length != 2)
        {
            throw new InvalidCallbackData($"{CallbackPrefix}_{{chat id}}_{{day}}", "didn't correct parts");
        }

        if (!long.TryParse(parts[0], out long chatId))
        {
            throw new InvalidCallbackData($"{CallbackPrefix}_{{chat id}}_{{day}}", "chat id wasn't a long");
        }

        if (!Day.TryParse(parts[1], out Day day, ignoreCase: true))
        {
            throw new InvalidCallbackData($"{CallbackPrefix}_{{chat id}}_{{day}}", $"day wasn't a {nameof(Day)}");
        }

        return (chatId, day);
    }
}