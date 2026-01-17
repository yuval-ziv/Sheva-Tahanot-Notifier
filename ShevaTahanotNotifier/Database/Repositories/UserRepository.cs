using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Repositories.Abstract;

namespace ShevaTahanotNotifier.Database.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(NotifierContext context) : base(context)
    {
    }
}