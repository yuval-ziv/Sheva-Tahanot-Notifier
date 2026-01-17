using ShevaTahanotNotifier.Telegram.CommandHandlers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram;

public class ShevaTahanotNotifierUpdateHandler : IUpdateHandler
{
    private readonly ILogger<ShevaTahanotNotifierUpdateHandler> _logger;
    private readonly Dictionary<string, ICommandHandler> _commandToCommandHandler;
    private readonly HelpCommandHandler _defaultCommandHandler;

    public ShevaTahanotNotifierUpdateHandler(ILogger<ShevaTahanotNotifierUpdateHandler> logger, IEnumerable<ICommandHandler> commandHandlers, HelpCommandHandler defaultCommandHandler)
    {
        _logger = logger;
        _commandToCommandHandler = commandHandlers.ToDictionary(commandHandler => commandHandler.Command, commandHandler => commandHandler);
        _defaultCommandHandler = defaultCommandHandler;
        _commandToCommandHandler[_defaultCommandHandler.Command] = _defaultCommandHandler;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessageAsync(message, cancellationToken),
            { CallbackQuery: { } callbackQuery } => OnCallbackAsync(callbackQuery, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(update), update, null)
        });
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Encountered an error while handling update. Error Source {ErrorSource}", source);
        return Task.CompletedTask;
    }

    private async Task OnMessageAsync(Message msg, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Receive {MessageType} message", msg.Type);
        if (msg.Text is not { } messageText)
            return;

        string command = messageText.Split(' ')[0];
        ICommandHandler commandHandler = _commandToCommandHandler.GetValueOrDefault(command, _defaultCommandHandler);
        Message sentMessage = await commandHandler.HandleCommandAsync(msg, cancellationToken);

        _logger.LogDebug("Send response with id {SentMessageId} using {CommandHandler}", sentMessage.Id, commandHandler.GetType().Name);
    }

    private async Task OnCallbackAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}