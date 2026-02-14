using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Repositories.Abstract;

namespace ShevaTahanotNotifier.Database.Repositories;

public interface IConversationRepository : IGenericRepository<Conversation>
{
    Task<Conversation?> GetByChatIdAsync(long chatId, bool tracking = false, CancellationToken cancellationToken = default);
}