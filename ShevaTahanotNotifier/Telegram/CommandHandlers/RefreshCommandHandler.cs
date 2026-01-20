using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Services;
using ShevaTahanotNotifier.Telegram.CommandHandlers.Abstraction;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram.CommandHandlers;

public class RefreshCommandHandler : AbstractAdminCommandHandler, ICommandHandler
{
    private const bool ManualRefresh = true;
    private readonly IBridgeStatusService _bridgeStatusService;

    public RefreshCommandHandler(ILogger<RefreshCommandHandler> logger, ITelegramBotClient bot, IAdminUserValidatorService adminUserValidatorService, IBridgeStatusService bridgeStatusService)
        : base(logger, bot, adminUserValidatorService)
    {
        _bridgeStatusService = bridgeStatusService;
    }

    public override string Command => "/refresh";
    public override string Description => "refresh bridge status cache (admin only)";

    protected override async Task<Message> HandleAuthenticatedAdminCommandAsync(Message message, CancellationToken cancellationToken = default)
    {
        long chatId = message.Chat.Id;
        Logger.LogDebug("Handling refresh command from {ChatId}", chatId);
        BridgeStatus bridgeStatus = await _bridgeStatusService.UpdateBridgeStatusAsync(ManualRefresh, cancellationToken);
        return await Bot.SendMessage(chatId, $"Bridge status has been refreshed. {bridgeStatus.ToNotificationString()}", cancellationToken: cancellationToken);
    }
}