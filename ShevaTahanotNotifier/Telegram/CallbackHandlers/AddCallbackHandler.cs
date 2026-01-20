using ShevaTahanotNotifier.Database.Entities.Enums;
using ShevaTahanotNotifier.Database.Repositories;
using ShevaTahanotNotifier.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram.CallbackHandlers;

public class AddCallbackHandler : ICallbackHandler
{
    private readonly ILogger<AddCallbackHandler> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly ITelegramUserRepository _telegramUserRepository;

    public AddCallbackHandler(ILogger<AddCallbackHandler> logger, ITelegramBotClient bot, ITelegramUserRepository telegramUserRepository)
    {
        _logger = logger;
        _bot = bot;
        _telegramUserRepository = telegramUserRepository;
    }

    public string CallbackPrefix => "add";

    public async Task<Message> HandleCallbackAsync(CallbackQuery callback, CancellationToken cancellationToken = default)
    {
        Message? message = callback.Message;
        ArgumentNullException.ThrowIfNull(message);

        long? chatId = message.Chat.Id;
        int? messageId = message.MessageId;

        await _bot.EditMessageReplyMarkup(chatId: chatId, messageId: messageId.Value, replyMarkup: null, cancellationToken: cancellationToken);

        _logger.LogDebug("Handling register command from {ChatId}", chatId);

        if (!await _telegramUserRepository.ExistsByChatIdAsync(chatId.Value, cancellationToken))
        {
            _logger.LogDebug("User is not registered with chat {ChatId}", chatId);
            return await _bot.SendMessage(chatId, $"You are not registered! Please register and try again.", cancellationToken: cancellationToken);
        }

        (long callbackDataChatId, Day day) = ParseCallbackData(callback.Data);

        if (callbackDataChatId != chatId)
        {
            throw new InvalidCallbackChatId(chatId.Value, callbackDataChatId);
        }

        _logger.LogDebug("User from chat id {ChatId} wants to create a notification on day {Day}", chatId, day);
        return await _bot.EditMessageText(chatId: chatId, messageId: messageId.Value, text: $"You chose {day.ToStringFast()}. At what time? Use 24-hour format (e.g. 17:30).",
            cancellationToken: cancellationToken);
    }

    private (long callbackDataChatId, Day day) ParseCallbackData(string? callbackData)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(callbackData);
        string[] parts = callbackData.Split("_");
        if (parts.Length != 3)
        {
            throw new InvalidCallbackData("add_{chat id}_{day}", "didn't get 3 parts");
        }

        if (!long.TryParse(parts[1], out long chatId))
        {
            throw new InvalidCallbackData("add_{chat id}_{day}", "chat id wasn't a long");
        }

        if (!Day.TryParse(parts[2], out Day day, ignoreCase: true))
        {
            throw new InvalidCallbackData("add_{chat id}_{day}", $"day wasn't a {nameof(Day)}");
        }

        return (chatId, day);
    }
}