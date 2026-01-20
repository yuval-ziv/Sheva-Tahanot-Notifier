using ShevaTahanotNotifier.Telegram.CommandHandlers.Abstraction;
using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram.CommandHandlers;

public class DisableNotificationScheduleCommandHandler : ICommandHandler
{
    public string Command => "/disable";
    public string Description => "disables a notification schedule";

    public Task<Message> HandleCommandAsync(Message message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}