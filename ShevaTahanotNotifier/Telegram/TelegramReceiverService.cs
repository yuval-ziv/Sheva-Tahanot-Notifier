using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram;

public class TelegramReceiverService : ITelegramReceiverService
{
    private readonly ILogger<TelegramReceiverService> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IUpdateHandler _updateHandler;
    private readonly IBotCommandHelper _botCommandHelper;

    public TelegramReceiverService(ILogger<TelegramReceiverService> logger, ITelegramBotClient botClient, IUpdateHandler updateHandler, IBotCommandHelper botCommandHelper)
    {
        _logger = logger;
        _botClient = botClient;
        _updateHandler = updateHandler;
        _botCommandHelper = botCommandHelper;
    }

    public async Task ReceiveAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions { DropPendingUpdates = false, AllowedUpdates = [] };

        User me = await _botClient.GetMe(stoppingToken);
        _logger.LogInformation("Start receiving updates for {BotName}", me.Username ?? "Shave Tahanot Notifier");

        await RegisterCommandsAsync(stoppingToken);

        await _botClient.ReceiveAsync(_updateHandler, receiverOptions, stoppingToken);
    }

    private Task RegisterCommandsAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<BotCommand> botCommands = _botCommandHelper.GetAll();
        return _botClient.SetMyCommands(botCommands, cancellationToken: cancellationToken);
    }
}