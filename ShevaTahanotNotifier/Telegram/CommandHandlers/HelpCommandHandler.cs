using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ShevaTahanotNotifier.Telegram.CommandHandlers;

public class HelpCommandHandler : ICommandHandler
{
    private const char Space = ' ';
    private const char CodeBlock = '`';
    private const int PaddingBetweenCommandAndDescription = 12;
    private const string? Separator = @" \- ";
    private const int PreCommandPadding = 4;
    private readonly ITelegramBotClient _bot;
    private readonly IBotCommandHelper _botCommandHelper;

    public HelpCommandHandler(ITelegramBotClient bot, IEnumerable<ICommandHandler> commandHandlers)
    {
        _bot = bot;
        _botCommandHelper = new BotCommandHelper(commandHandlers, this);
    }

    public string Command => "/help";
    public string Description => "shows commands help";

    public async Task<Message> HandleCommandAsync(Message message, CancellationToken cancellationToken = default)
    {
        var helpMessage = new StringBuilder();
        helpMessage.Append("*__Bot menu__*").AppendLine();
        foreach (BotCommand command in _botCommandHelper.GetAll())
        {
            int padding = PaddingBetweenCommandAndDescription - command.Command.Length;

            helpMessage.Append(Space, PreCommandPadding);
            helpMessage.Append(CodeBlock);
            helpMessage.Append(command.Command);
            helpMessage.Append(CodeBlock);
            helpMessage.Append(CodeBlock);
            helpMessage.Append(Space, padding);
            helpMessage.Append(CodeBlock);
            
            helpMessage.Append(Separator);
            helpMessage.Append(command.Description);
            
            helpMessage.AppendLine();
        }

        return await _bot.SendMessage(message.Chat, helpMessage.ToString(), parseMode: ParseMode.MarkdownV2, replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
    }
}