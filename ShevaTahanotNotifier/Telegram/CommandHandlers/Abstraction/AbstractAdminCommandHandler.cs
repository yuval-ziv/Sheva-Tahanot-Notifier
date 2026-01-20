using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram.CommandHandlers.Abstraction;

public abstract class AbstractAdminCommandHandler : IAdminCommandHandler
{
    protected readonly ILogger<AbstractAdminCommandHandler> Logger;
    protected readonly ITelegramBotClient Bot;
    private readonly IAdminUserValidatorService _adminUserValidatorService;

    protected AbstractAdminCommandHandler(ILogger<AbstractAdminCommandHandler> logger, ITelegramBotClient bot, IAdminUserValidatorService adminUserValidatorService)
    {
        Logger = logger;
        Bot = bot;
        _adminUserValidatorService = adminUserValidatorService;
    }

    public abstract string Command { get; }
    public abstract string Description { get; }

    public async Task<Message> HandleCommandAsync(Message message, CancellationToken cancellationToken = default)
    {
        long chatId = message.Chat.Id;
        Logger.LogDebug("Handling admin command from {ChatId}", chatId);

        if (_adminUserValidatorService.IsAdmin(chatId))
        {
            Logger.LogDebug("User from chat {ChatId} is an admin", chatId);
            return await HandleAuthenticatedAdminCommandAsync(message, cancellationToken: cancellationToken);
        }

        Logger.LogWarning("User from chat {ChatId} is not an admin!", chatId);
        return await Bot.SendMessage(chatId, "You are not authenticated as admin. You are not allowed to use this command", cancellationToken: cancellationToken);
    }

    protected abstract Task<Message> HandleAuthenticatedAdminCommandAsync(Message message, CancellationToken cancellationToken = default);
}