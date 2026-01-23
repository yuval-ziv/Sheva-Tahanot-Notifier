using ShevaTahanotNotifier.Database.Entities;
using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram.ConversationHandlers;

public interface IConversationHandler
{
    public string StepName { get; }
    public Task<Message> HandleConversationAsync(Message message, Conversation conversation, CancellationToken cancellationToken = default);
}