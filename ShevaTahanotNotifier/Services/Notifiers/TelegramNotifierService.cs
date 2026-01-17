using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Entities.NotificationProviderConfiguration;
using Telegram.Bot;

namespace ShevaTahanotNotifier.Services.Notifiers;

public class TelegramNotifierService : BaseNotifierService<TelegramNotificationProviderConfiguration>
{
    private readonly ILogger<TelegramNotifierService> _logger;
    private readonly ITelegramBotClient _bot;

    public TelegramNotifierService(ILogger<TelegramNotifierService> logger, ITelegramBotClient bot)
    {
        _logger = logger;
        _bot = bot;
    }

    public override NotificationProvider NotificationProvider => NotificationProvider.Telegram;

    public override async Task NotifyAsync(User user, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Notifying user with id {UserId}", user.Id);
        TelegramNotificationProviderConfiguration configuration = GetConfiguration(user);
        await _bot.SendMessage(text: "Notification", chatId: configuration.ChatId, cancellationToken: cancellationToken);
    }
}