using ShevaTahanotNotifier.Telegram.CallbackHandlers;
using ShevaTahanotNotifier.Telegram.CommandHandlers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram;

public class ShevaTahanotNotifierUpdateHandler : IUpdateHandler
{
    private readonly ILogger<ShevaTahanotNotifierUpdateHandler> _logger;
    private readonly Dictionary<string, ICommandHandler> _commandToCommandHandler;
    private readonly List<ICallbackHandler> _callbackHandlers;
    private readonly HelpCommandHandler _defaultCommandHandler;

    public ShevaTahanotNotifierUpdateHandler(ILogger<ShevaTahanotNotifierUpdateHandler> logger, IEnumerable<ICommandHandler> commandHandlers, HelpCommandHandler defaultCommandHandler,
        IEnumerable<ICallbackHandler> callbackHandlers)
    {
        _logger = logger;
        _commandToCommandHandler = commandHandlers.ToDictionary(commandHandler => commandHandler.Command, commandHandler => commandHandler);
        _defaultCommandHandler = defaultCommandHandler;
        _commandToCommandHandler[_defaultCommandHandler.Command] = _defaultCommandHandler;
        _callbackHandlers = callbackHandlers.ToList();
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

    private async Task OnMessageAsync(Message message, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Receive {MessageType} message", message.Type);
        if (message.Text is not { } messageText)
        {
            return;
        }

        string command = messageText.Split(' ')[0];
        ICommandHandler commandHandler = _commandToCommandHandler.GetValueOrDefault(command, _defaultCommandHandler);
        Message sentMessage = await commandHandler.HandleCommandAsync(message, cancellationToken);

        _logger.LogDebug("Send response with id {SentMessageId} using {CommandHandler}", sentMessage.Id, commandHandler.GetType().Name);
    }

    private async Task OnCallbackAsync(CallbackQuery callback, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Receive callback with data {CallbackData}", callback.Data);
        if (callback.Data is not { } callbackData)
            return;

        ICallbackHandler? callbackHandler = _callbackHandlers.FirstOrDefault(callbackHandler => callbackData.StartsWith(callbackHandler.CallbackPrefix, StringComparison.InvariantCultureIgnoreCase));

        if (callbackHandler is null)
        {
            _logger.LogWarning("Couldn't find callback handler for  {CallbackData}", callback.Data);
            return;
        }

        Message sentMessage = await callbackHandler.HandleCallbackAsync(callback, cancellationToken);
        _logger.LogDebug("Send response with id {SentMessageId} using {CommandHandler}", sentMessage.Id, callbackHandler.GetType().Name);
    }
}