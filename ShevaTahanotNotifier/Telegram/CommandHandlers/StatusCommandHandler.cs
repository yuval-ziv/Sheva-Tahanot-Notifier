using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Repositories;
using ShevaTahanotNotifier.Services;
using ShevaTahanotNotifier.Telegram.CommandHandlers.Abstraction;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram.CommandHandlers;

public class StatusCommandHandler : ICommandHandler
{
    private readonly ILogger<StatusCommandHandler> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly ITelegramUserRepository _telegramUserRepository;
    private readonly IBridgeStatusService _bridgeStatusService;

    public StatusCommandHandler(ILogger<StatusCommandHandler> logger, ITelegramBotClient bot, ITelegramUserRepository telegramUserRepository, IBridgeStatusService bridgeStatusService)
    {
        _logger = logger;
        _bot = bot;
        _telegramUserRepository = telegramUserRepository;
        _bridgeStatusService = bridgeStatusService;
    }

    public string Command => "/status";
    public string Description => "get current bridge status";

    public async Task<Message> HandleCommandAsync(Message message, CancellationToken cancellationToken = default)
    {
        long chatId = message.Chat.Id;
        _logger.LogDebug("Handling status command from {ChatId}", chatId);

        if (!await _telegramUserRepository.ExistsByChatIdAsync(chatId, cancellationToken))
        {
            _logger.LogDebug("User is not registered with chat {ChatId}", chatId);
            return await _bot.SendMessage(chatId, $"You are not registered.", cancellationToken: cancellationToken);
        }

        BridgeStatus bridgeStatus = await _bridgeStatusService.GetLastBridgeStatusAsync(cancellationToken: cancellationToken);

        return await _bot.SendMessage(chatId, bridgeStatus.ToNotificationString(), cancellationToken: cancellationToken);
    }
}