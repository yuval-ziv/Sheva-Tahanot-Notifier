using System.Text;
using ShevaTahanotNotifier.Telegram.CommandHandlers.Abstraction;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ShevaTahanotNotifier.Telegram.CommandHandlers;

public class HelpCommandHandler : ICommandHandler
{
    private const char Space = ' ';
    private const string CodeBlockStart = "<code>";
    private const string CodeBlockEnd = "</code>";
    private const int PaddingBetweenCommandAndDescription = 12;
    private const string? Separator = " - ";
    private const int PreCommandPadding = 4;
    private readonly ITelegramBotClient _bot;
    private readonly IBotCommandHelper _botCommandHelper;
    private readonly IAdminUserValidatorService _adminUserValidatorService;

    public HelpCommandHandler(ITelegramBotClient bot, IEnumerable<ICommandHandler> commandHandlers, IAdminUserValidatorService adminUserValidatorService)
    {
        _bot = bot;
        _botCommandHelper = new BotCommandHelper(commandHandlers, this);
        _adminUserValidatorService = adminUserValidatorService;
    }

    public string Command => "/help";
    public string Description => "shows commands help";

    public async Task<Message> HandleCommandAsync(Message message, CancellationToken cancellationToken = default)
    {
        long chatId = message.Chat.Id;
        var helpMessage = new StringBuilder();
        helpMessage.Append("<b><u>Bot menu</u></b>").AppendLine();
        bool isAdmin = _adminUserValidatorService.IsAdmin(chatId);
        foreach (BotCommand command in _botCommandHelper.GetAll(isAdmin))
        {
            int padding = PaddingBetweenCommandAndDescription - command.Command.Length;

            helpMessage.Append(Space, PreCommandPadding);
            helpMessage.Append(CodeBlockStart);
            helpMessage.Append(command.Command);
            helpMessage.Append(CodeBlockEnd);
            helpMessage.Append(CodeBlockStart);
            helpMessage.Append(Space, padding);
            helpMessage.Append(CodeBlockEnd);

            helpMessage.Append(Separator);
            helpMessage.Append(command.Description);

            helpMessage.AppendLine();
        }

        return await _bot.SendMessage(chatId, helpMessage.ToString(), parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
    }
}