namespace ShevaTahanotNotifier.Telegram.CommandHandlers.Abstraction;

public interface IAdminUserValidatorService
{
    bool IsAdmin(long chatId);
}