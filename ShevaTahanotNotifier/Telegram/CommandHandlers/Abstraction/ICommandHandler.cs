using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram.CommandHandlers.Abstraction;

public interface ICommandHandler
{
    string Command { get; }

    string Description { get; }
    Task<Message> HandleCommandAsync(Message message, CancellationToken cancellationToken = default);
}