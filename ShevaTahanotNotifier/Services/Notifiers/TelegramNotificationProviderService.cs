using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Entities.Enums;
using ShevaTahanotNotifier.Database.Entities.NotificationProviderConfiguration;
using Telegram.Bot;

namespace ShevaTahanotNotifier.Services.Notifiers;

public class TelegramNotificationProviderService : BaseNotificationProviderService<TelegramNotificationProviderConfiguration>
{
    private readonly ILogger<TelegramNotificationProviderService> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly IBridgeStatusService _bridgeStatusService;

    public TelegramNotificationProviderService(ILogger<TelegramNotificationProviderService> logger, ITelegramBotClient bot, IBridgeStatusService bridgeStatusService)
    {
        _logger = logger;
        _bot = bot;
        _bridgeStatusService = bridgeStatusService;
    }

    public override NotificationProvider NotificationProvider => NotificationProvider.Telegram;

    public override async Task NotifyAsync(User user, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Notifying user with id {UserId}", user.Id);
        TelegramNotificationProviderConfiguration configuration = GetConfiguration(user);
        BridgeStatus bridgeStatus = await _bridgeStatusService.GetLastBridgeStatusAsync(cancellationToken: cancellationToken);
        await _bot.SendMessage(text: bridgeStatus.ToNotificationString(), chatId: configuration.ChatId, cancellationToken: cancellationToken);
    }
}