using Microsoft.Extensions.Options;
using ShevaTahanotNotifier.Configuration;

namespace ShevaTahanotNotifier.Telegram.CommandHandlers.Abstraction;

public interface IAdminUserValidatorService
{
    bool IsAdmin(long chatId);
}

public class AdminUserValidatorService : IAdminUserValidatorService
{
    private readonly TelegramBotOptions _options;

    public AdminUserValidatorService(IOptionsMonitor<TelegramBotOptions> optionsMonitor)
    {
        _options = optionsMonitor.CurrentValue;
    }

    public bool IsAdmin(long chatId)
    {
        return _options.AdminChatIds.Contains(chatId);
    }
}