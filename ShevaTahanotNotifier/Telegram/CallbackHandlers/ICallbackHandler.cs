using ShevaTahanotNotifier.Database.Entities;
using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram.CallbackHandlers;

public interface ICallbackHandler
{
    string CallbackPrefix { get; }
    Task<(Message Message, Conversation? Conversation)> HandleCallbackAsync(CallbackQuery callback, CancellationToken cancellationToken = default);
}