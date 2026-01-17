using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Entities.NotificationProviderConfiguration;
using ShevaTahanotNotifier.Database.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = ShevaTahanotNotifier.Database.Entities.User;

namespace ShevaTahanotNotifier.Telegram.CommandHandlers;

public class RegisterCommandHandler : ICommandHandler
{
    private readonly ILogger<RegisterCommandHandler> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly ITelegramUserRepository _telegramUserRepository;

    public RegisterCommandHandler(ILogger<RegisterCommandHandler> logger, ITelegramBotClient bot, ITelegramUserRepository telegramUserRepository)
    {
        _logger = logger;
        _bot = bot;
        _telegramUserRepository = telegramUserRepository;
    }

    public string Command => "/register";
    public string Description => "register chat to notifications";

    public async Task<Message> HandleCommandAsync(Message message, CancellationToken cancellationToken = default)
    {
        long chatId = message.Chat.Id;

        _logger.LogDebug("Handling register command from {ChatId}", chatId);

        if (await _telegramUserRepository.ExistsByChatIdAsync(chatId, cancellationToken))
        {
            _logger.LogDebug("User is already registered with chat {ChatId}", chatId);
            return await _bot.SendMessage(chatId, $"User {message.From?.Username} is already registered", cancellationToken: cancellationToken);
        }

        _logger.LogDebug("Creating Telegram user for chat id {ChatId}", chatId);
        await CreateUserAsync(message, cancellationToken);
        _logger.LogDebug("Created Telegram user for chat id {ChatId}", chatId);
        return await _bot.SendMessage(chatId, $"User {message.From?.Username} has been registered! Now add new notifications.", cancellationToken: cancellationToken);
    }

    private Task<User> CreateUserAsync(Message message, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            Provider = NotificationProvider.Telegram,
            Configuration = new TelegramNotificationProviderConfiguration
            {
                ChatId = message.Chat.Id,
            }
        };

        return _telegramUserRepository.CreateAsync(user, cancellationToken: cancellationToken);
    }
}