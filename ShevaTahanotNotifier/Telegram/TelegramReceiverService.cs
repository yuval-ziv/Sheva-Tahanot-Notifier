using Microsoft.Extensions.Options;
using ShevaTahanotNotifier.Configuration;
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
    private readonly TelegramBotOptions _options;

    public TelegramReceiverService(ILogger<TelegramReceiverService> logger, ITelegramBotClient botClient, IUpdateHandler updateHandler, IBotCommandHelper botCommandHelper,
        IOptionsMonitor<TelegramBotOptions> optionsMonitor)
    {
        _logger = logger;
        _botClient = botClient;
        _updateHandler = updateHandler;
        _botCommandHelper = botCommandHelper;
        _options = optionsMonitor.CurrentValue;
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
        IEnumerable<BotCommand> regularBotCommands = _botCommandHelper.GetAll(false);
        Task setNonAdminCommandsTask = _botClient.SetMyCommands(regularBotCommands, scope: new BotCommandScopeDefault(), cancellationToken: cancellationToken);

        IEnumerable<BotCommand> allBotCommands = _botCommandHelper.GetAll(true);
        IEnumerable<Task> setAdminCommandsTasks = _options.AdminChatIds.Select(adminChatId => SetNonAdminCommandsAsync(allBotCommands, adminChatId, cancellationToken));

        return Task.WhenAll(setAdminCommandsTasks.Append(setNonAdminCommandsTask));
    }

    private Task SetNonAdminCommandsAsync(IEnumerable<BotCommand> allBotCommands, long adminChatId, CancellationToken cancellationToken)
    {
        return _botClient.SetMyCommands(allBotCommands, scope: new BotCommandScopeChat { ChatId = adminChatId }, cancellationToken: cancellationToken);
    }
}