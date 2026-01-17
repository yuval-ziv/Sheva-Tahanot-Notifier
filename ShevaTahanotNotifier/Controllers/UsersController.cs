using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShevaTahanotNotifier.Database;
using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Entities.NotificationProviderConfiguration;
using ShevaTahanotNotifier.Services.Notifiers;

namespace ShevaTahanotNotifier.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UsersController : ControllerBase
{
    private readonly NotifierContext _context;
    private readonly List<INotifierService> _notifierServices;

    public UsersController(NotifierContext context, IEnumerable<INotifierService> notifierServices)
    {
        _context = context;
        _notifierServices = notifierServices.ToList();
    }

    [HttpGet]
    public async Task<List<User>> GetAll()
    {
        var users = await _context.Users.ToListAsync();
        return users;
    }

    [HttpGet]
    public async Task<User> Create()
    {
        var user = GetRandomUser();
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    [HttpGet]
    public async Task<User?> Notify(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is not null)
        {
            await Task.WhenAll(_notifierServices.Select(notifierService => notifierService.NotifyAsync(user)));
        }

        return user;
    }

    private static User GetRandomUser()
    {
        var useTelegram = Random.Shared.Next(0, 2) == 0;
        var user = new User
        {
            Provider = useTelegram ? NotificationProvider.Telegram : NotificationProvider.Email,
            Configuration = null,
            NotificationSchedules = null
        };
        if (useTelegram)
        {
            user.Configuration = new TelegramNotificationProviderConfiguration
            {
                UserId = user.Id,
                ChatId = 12345L,
            };
        }
        else
        {
            user.Configuration = new EmailNotificationProviderConfiguration
            {
                UserId = user.Id,
                EmailAddress = "test@test.com",
            };
        }

        user.NotificationSchedules =
        [
            new NotificationSchedule
            {
                UserId = user.Id,
            },
        ];
        return user;
    }
}