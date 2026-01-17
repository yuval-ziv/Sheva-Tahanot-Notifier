using ShevaTahanotNotifier.ExtensionMethods;
using ShevaTahanotNotifier.Telegram.CommandHandlers;
using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram;

public class BotCommandHelper : IBotCommandHelper
{
    private readonly ICollection<ICommandHandler> _commandHandlers;
    private readonly HelpCommandHandler _helpCommandHandler;

    public BotCommandHelper(IEnumerable<ICommandHandler> commandHandlers, HelpCommandHandler helpCommandHandler)
    {
        _commandHandlers = commandHandlers.ToCollection();
        _helpCommandHandler = helpCommandHandler;
    }

    public IEnumerable<BotCommand> GetAll()
    {
        return _commandHandlers.Prepend(_helpCommandHandler).Select(GetBotCommand);
    }

    private BotCommand GetBotCommand(ICommandHandler commandHandler)
    {
        return new BotCommand
        {
            Command = commandHandler.Command,
            Description = commandHandler.Description,
        };
    }
}