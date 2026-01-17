using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram;

public interface IBotCommandHelper
{
    IEnumerable<BotCommand> GetAll();
}