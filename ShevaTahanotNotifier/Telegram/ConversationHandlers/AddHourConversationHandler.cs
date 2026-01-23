using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Entities.Enums;
using ShevaTahanotNotifier.Database.Repositories;
using ShevaTahanotNotifier.Telegram.CallbackHandlers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram.ConversationHandlers;

public class AddHourConversationHandler : IConversationHandler
{
    private readonly ILogger<AddHourConversationHandler> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly IConversationRepository _conversationRepository;
    private readonly INotificationScheduleRepository _notificationScheduleRepository;

    public const string AddHourStepName = "add_hour";

    public AddHourConversationHandler(ILogger<AddHourConversationHandler> logger, ITelegramBotClient bot, IConversationRepository conversationRepository,
        INotificationScheduleRepository notificationScheduleRepository)
    {
        _logger = logger;
        _bot = bot;
        _conversationRepository = conversationRepository;
        _notificationScheduleRepository = notificationScheduleRepository;
    }

    public string StepName => AddHourStepName;

    public async Task<Message> HandleConversationAsync(Message message, Conversation conversation, CancellationToken cancellationToken = default)
    {
        long chatId = message.Chat.Id;

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

        await _conversationRepository.DeleteAsync(conversation, saveChanges: false, cancellationToken: cancellationToken);
        await _notificationScheduleRepository.CreateAsync(notificationSchedule, saveChanges: true, cancellationToken);

        string every = day == Day.Everyday ? "" : " every";
        string dayString = day.ToStringFast();

        return await _bot.SendMessage(chatId, $"Successfully added new notification{every} {dayString} at {hour:00}:{minute:00}.", cancellationToken: cancellationToken);
    }

    private async Task<Message> DeleteConversationAndSendRestartProcessAsync(Conversation conversation, CancellationToken cancellationToken, long chatId)
    {
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