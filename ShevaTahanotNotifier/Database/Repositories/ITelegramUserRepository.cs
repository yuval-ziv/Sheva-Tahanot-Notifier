using ShevaTahanotNotifier.Database.Entities;

namespace ShevaTahanotNotifier.Database.Repositories;

public interface ITelegramUserRepository : IUserRepository
{
    IQueryable<User> GetAllByChatId(long chatId, bool tracking = false);
    Task<bool> ExistsByChatIdAsync(long chatId, CancellationToken cancellationToken = default);
}