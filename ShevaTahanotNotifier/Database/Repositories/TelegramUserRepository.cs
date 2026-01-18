using Microsoft.EntityFrameworkCore;
using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Entities.Enums;
using ShevaTahanotNotifier.Database.Entities.NotificationProviderConfiguration;

namespace ShevaTahanotNotifier.Database.Repositories;

public class TelegramUserRepository : UserRepository, ITelegramUserRepository
{
    public TelegramUserRepository(NotifierContext context) : base(context)
    {
    }


    public IQueryable<User> GetAllByChatId(long chatId, bool tracking = false)
    {
        return GetAll(tracking)
            .Where(user => user.Provider == NotificationProvider.Telegram)
            .Where(user => ((TelegramNotificationProviderConfiguration)user.Configuration!).ChatId == chatId);
    }

    public Task<bool> ExistsByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return GetAll()
            .Where(user => user.Provider == NotificationProvider.Telegram)
            .AnyAsync(user => ((TelegramNotificationProviderConfiguration)user.Configuration!).ChatId == chatId, cancellationToken: cancellationToken);
    }
}