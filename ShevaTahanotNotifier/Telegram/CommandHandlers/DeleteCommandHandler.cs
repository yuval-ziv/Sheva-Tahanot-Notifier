using Microsoft.EntityFrameworkCore;
using ShevaTahanotNotifier.Database.Repositories;
using ShevaTahanotNotifier.Telegram.CommandHandlers.Abstraction;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = ShevaTahanotNotifier.Database.Entities.User;

namespace ShevaTahanotNotifier.Telegram.CommandHandlers;

public class DeleteCommandHandler : ICommandHandler
{
    private readonly ILogger<DeleteCommandHandler> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly ITelegramUserRepository _telegramUserRepository;

    public DeleteCommandHandler(ILogger<DeleteCommandHandler> logger, ITelegramBotClient bot, ITelegramUserRepository telegramUserRepository)
    {
        _logger = logger;
        _bot = bot;
        _telegramUserRepository = telegramUserRepository;
    }

    public string Command => "/delete";
    public string Description => "delete chat and all related data";

    public async Task<Message> HandleCommandAsync(Message message, CancellationToken cancellationToken = default)
    {
        long chatId = message.Chat.Id;

        _logger.LogDebug("Handling delete command from {ChatId}", chatId);
        List<User> users = await _telegramUserRepository.GetAllByChatId(chatId).ToListAsync(cancellationToken);
        if (users.Count == 0)
        {
            _logger.LogDebug("No users found with chat id {ChatId}", chatId);
            return await _bot.SendMessage(chatId, $"User {message.From?.Username} has already been deleted.", cancellationToken: cancellationToken);
        }

        _logger.LogDebug("Deleting {Count} users with chat id {ChatId}", users.Count, chatId);
        await _telegramUserRepository.DeleteAsync(users, cancellationToken: cancellationToken);
        _logger.LogDebug("Deleted {Count} users with chat id {ChatId}", users.Count, chatId);
        return await _bot.SendMessage(chatId, $"User {message.From?.Username} and all related data have been successfully deleted! Goodbye :)", cancellationToken: cancellationToken);
    }
}