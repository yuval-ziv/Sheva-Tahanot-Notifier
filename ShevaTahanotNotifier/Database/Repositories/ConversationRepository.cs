using Microsoft.EntityFrameworkCore;
using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Repositories.Abstract;

namespace ShevaTahanotNotifier.Database.Repositories;

public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
{
    public ConversationRepository(NotifierContext context) : base(context)
    {
    }

    public Task<Conversation?> GetByChatIdAsync(long chatId, bool tracking = false, CancellationToken cancellationToken = default)
    {
        return GetAll(tracking).FirstOrDefaultAsync(conversation => conversation.ChatId == chatId, cancellationToken);
    }
}