using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram.ConversationHandlers;

public interface IFreeTextMessageHandler
{
    public Task<Message> HandleFreeTextMessageAsync(Message message, CancellationToken cancellationToken = default);
}