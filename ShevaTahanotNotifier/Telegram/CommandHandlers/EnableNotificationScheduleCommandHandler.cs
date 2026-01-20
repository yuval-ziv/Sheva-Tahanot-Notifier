using ShevaTahanotNotifier.Telegram.CommandHandlers.Abstraction;
using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram.CommandHandlers;

public class EnableNotificationScheduleCommandHandler : ICommandHandler
{
    public string Command => "/enable";
    public string Description => "enables a notification schedule";

    public Task<Message> HandleCommandAsync(Message message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}