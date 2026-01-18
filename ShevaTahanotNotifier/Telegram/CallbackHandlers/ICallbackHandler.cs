using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram.CallbackHandlers;

public interface ICallbackHandler
{
    string CallbackPrefix { get; }
    Task<Message> HandleCallbackAsync(CallbackQuery callback, CancellationToken cancellationToken = default);
}