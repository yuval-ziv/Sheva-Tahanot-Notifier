using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Repositories.Abstract;

namespace ShevaTahanotNotifier.Database.Repositories;

public class BridgeStatusRepository : GenericRepository<BridgeStatus>, IBridgeStatusRepository
{
    public BridgeStatusRepository(NotifierContext context) : base(context)
    {
    }
}