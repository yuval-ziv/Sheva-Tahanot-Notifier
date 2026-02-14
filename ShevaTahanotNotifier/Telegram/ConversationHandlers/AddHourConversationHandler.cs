using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Entities.Enums;
using ShevaTahanotNotifier.Database.Repositories;
using ShevaTahanotNotifier.Services;
using ShevaTahanotNotifier.Telegram.CallbackHandlers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram.ConversationHandlers;

public class AddHourConversationHandler : IConversationHandler
{
    private readonly ILogger<AddHourConversationHandler> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly IConversationRepository _conversationRepository;
    private readonly INotificationScheduleService _notificationScheduleService;

    public const string AddHourStepName = "add_hour";

    public AddHourConversationHandler(ILogger<AddHourConversationHandler> logger, ITelegramBotClient bot, IConversationRepository conversationRepository,
        INotificationScheduleService notificationScheduleService)
    {
        _logger = logger;
        _bot = bot;
        _conversationRepository = conversationRepository;
        _notificationScheduleService = notificationScheduleService;
    }

    public string StepName => AddHourStepName;

    public async Task<Message> HandleConversationAsync(Message message, Conversation conversation, CancellationToken cancellationToken = default)
    {
        long chatId = message.Chat.Id;
        _logger.LogDebug("Handling add hour conversation with chat {ChatId}", chatId);

        if (conversation.ExtraData is null)
        {
            return await DeleteConversationAndSendRestartProcessAsync(conversation, cancellationToken, chatId);
        }

        if (!conversation.ExtraData.TryGetValue(AddNotificationDayCallbackHandler.SelectedDayExtraDataKey, out string? selectedDayExtraData))
        {
            return await DeleteConversationAndSendRestartProcessAsync(conversation, cancellationToken, chatId);
        }

        if (!Day.TryParse(selectedDayExtraData, out Day day, ignoreCase: true))
        {
            return await DeleteConversationAndSendRestartProcessAsync(conversation, cancellationToken, chatId);
        }

        if (!TryParseTo24Hour(message.Text, out short hour, out short minute))
        {
            _logger.LogWarning("Unable to parse {MessageText} into 24-hour format from chat {ChatId}", message.Text, chatId);
            return await _bot.SendMessage(chatId, $"Unable to parse {message.Text} into 24-hour format. Please try again.", cancellationToken: cancellationToken);
        }

        var notificationSchedule = new NotificationSchedule
        {
            Day = day,
            Enabled = true,
            Hour = hour,
            Minute = minute,
            UserId = conversation.UserId,
        };

        _logger.LogDebug("Deleting conversation {ConversationId} from chat {ChatId} and creating a notification schedule", conversation.Id, chatId);
        await _conversationRepository.DeleteAsync(conversation, saveChanges: false, cancellationToken: cancellationToken);
        await _notificationScheduleService.CreateAsync(notificationSchedule, cancellationToken);
        _logger.LogDebug("Finished creating notification schedule with id {NotificationScheduleId} for chat {ChatId}", notificationSchedule.Id, chatId);

        string every = day == Day.Everyday ? "" : " every";
        string dayString = day.ToStringFast();

        return await _bot.SendMessage(chatId, $"Successfully added new notification{every} {dayString} at {hour:00}:{minute:00}.", cancellationToken: cancellationToken);
    }

    private async Task<Message> DeleteConversationAndSendRestartProcessAsync(Conversation conversation, CancellationToken cancellationToken, long chatId)
    {
        _logger.LogWarning("Unknown error occured when trying to handle add hour conversation with chat {ChatId}. Restarting process", chatId);
        await _conversationRepository.DeleteAsync(conversation, saveChanges: true, cancellationToken);
        return await _bot.SendMessage(chatId, "Unknown error occured. Please restart the process.", cancellationToken: cancellationToken);
    }

    private bool TryParseTo24Hour(string? text, out short hour, out short minute)
    {
        hour = -1;
        minute = -1;
        if (text is null)
        {
            return false;
        }

        string[] textParts = text.Split(':');
        if (textParts.Length != 2)
        {
            return false;
        }

        if (!short.TryParse(textParts[0], out hour))
        {
            return false;
        }

        if (!short.TryParse(textParts[1], out minute))
        {
            return false;
        }

        return true;
    }
}