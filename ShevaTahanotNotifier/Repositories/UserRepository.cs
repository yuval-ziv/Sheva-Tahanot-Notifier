using ShevaTahanotNotifier.Database;
using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Entities.NotificationProviderConfiguration;
using ShevaTahanotNotifier.Repositories.Abstract;

namespace ShevaTahanotNotifier.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(NotifierContext context) : base(context)
    {
    }
}