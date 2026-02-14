using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Repositories;
using ShevaTahanotNotifier.Exceptions;
using ShevaTahanotNotifier.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = ShevaTahanotNotifier.Database.Entities.User;

namespace ShevaTahanotNotifier.Telegram.CallbackHandlers;

public class DisableNotificationCallbackHandler : ICallbackHandler
{
    public const string CallbackName = "dn"; //disable notification

    private readonly ILogger<DisableNotificationCallbackHandler> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly ITelegramUserRepository _telegramUserRepository;
    private readonly INotificationScheduleService _notificationScheduleService;

    public DisableNotificationCallbackHandler(ILogger<DisableNotificationCallbackHandler> logger, ITelegramBotClient bot, ITelegramUserRepository telegramUserRepository,
        INotificationScheduleService notificationScheduleService)
    {
        _logger = logger;
        _bot = bot;
        _telegramUserRepository = telegramUserRepository;
        _notificationScheduleService = notificationScheduleService;
    }

    public string CallbackPrefix => CallbackName;

    public async Task<(Message Message, Conversation? Conversation)> HandleCallbackAsync(CallbackQuery callback, CancellationToken cancellationToken = default)
    {
        Message? message = callback.Message;
        ArgumentNullException.ThrowIfNull(message);

        long chatId = message.Chat.Id;
        int messageId = message.MessageId;

        await _bot.EditMessageReplyMarkup(chatId: chatId, messageId: messageId, replyMarkup: null, cancellationToken: cancellationToken);

        _logger.LogDebug("Handling disable notification callback from {ChatId}", chatId);

        User? user = await _telegramUserRepository.GetByChatIdAsync(chatId, tracking: false, cancellationToken);

        if (user is null)
        {
            _logger.LogDebug("User is not registered with chat {ChatId}", chatId);
            return (await _bot.SendMessage(chatId, "You are not registered! Please register and try again.", cancellationToken: cancellationToken), null);
        }

        (long callbackDataChatId, Guid notificationId) = ParseCallbackData(callback.Data);

        if (callbackDataChatId != chatId)
        {
            throw new InvalidCallbackChatId(chatId, callbackDataChatId);
        }

        if (notificationId == Guid.Empty)
        {
            _logger.LogDebug("Notification enabling was canceled for chat {ChatId}", chatId);
            return (await _bot.EditMessageText(chatId: chatId, messageId: messageId, text: "Canceled disable operation.", cancellationToken: cancellationToken), null);
        }

        _logger.LogDebug("Disabling notification {NotificationId}", notificationId);
        await _notificationScheduleService.DisableAsync(notificationId, cancellationToken);
        _logger.LogDebug("Notification {NotificationId} was disabled", notificationId);

        return (await _bot.EditMessageText(chatId: chatId, messageId: messageId, text: "Disabled notification.", cancellationToken: cancellationToken), null);
    }

    private (long callbackDataChatId, Guid notificationId) ParseCallbackData(string? callbackData)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(callbackData);
        string callbackActualData = callbackData.Replace(CallbackPrefix + "_", string.Empty);
        string[] parts = callbackActualData.Split("_");
        if (parts.Length != 2)
        {
            throw new InvalidCallbackData($"{CallbackPrefix}_{{chat id}}_{{notification id}}", "didn't correct parts");
        }

        if (!long.TryParse(parts[0], out long chatId))
        {
            throw new InvalidCallbackData($"{CallbackPrefix}_{{chat id}}_{{notification id}}", "chat id wasn't a long");
        }

        if (parts[1] == "cancel")
        {
            return (chatId, Guid.Empty);
        }

        if (!Guid.TryParse(parts[1], out Guid notificationId))
        {
            throw new InvalidCallbackData($"{CallbackPrefix}_{{chat id}}_{{notification id}}", $"notification id wasn't a {nameof(Guid)}");
        }

        return (chatId, notificationId);
    }
}