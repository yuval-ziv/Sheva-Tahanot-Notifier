using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Repositories;
using ShevaTahanotNotifier.Telegram.CommandHandlers;
using Telegram.Bot.Types;

namespace ShevaTahanotNotifier.Telegram.ConversationHandlers;

public class FreeTextMessageHandler : IFreeTextMessageHandler
{
    private readonly ILogger<FreeTextMessageHandler> _logger;
    private readonly IConversationRepository _conversationRepository;
    private readonly HelpCommandHandler _helpCommandHandler;
    private readonly Dictionary<string, IConversationHandler> _stepNameToConversationHandlers;

    public FreeTextMessageHandler(ILogger<FreeTextMessageHandler> logger, IConversationRepository conversationRepository, HelpCommandHandler helpCommandHandler,
        IEnumerable<IConversationHandler> conversationHandlers)
    {
        _logger = logger;
        _conversationRepository = conversationRepository;
        _helpCommandHandler = helpCommandHandler;
        _stepNameToConversationHandlers = conversationHandlers.ToDictionary(conversationHandler => conversationHandler.StepName, conversationHandler => conversationHandler);
    }

    public async Task<Message> HandleFreeTextMessageAsync(Message message, CancellationToken cancellationToken = default)
    {
        long chatId = message.Chat.Id;
        Conversation? conversation = await _conversationRepository.GetByChatIdAsync(chatId, tracking: true, cancellationToken: cancellationToken);

        if (conversation?.NextStep is null)
        {
            _logger.LogDebug("Got a free text message without prior conversation, forwarding to {HelpCommandHandler}", _helpCommandHandler.GetType().Name);
            return await _helpCommandHandler.HandleCommandAsync(message, cancellationToken);
        }

        if (!_stepNameToConversationHandlers.TryGetValue(conversation.NextStep, out IConversationHandler? conversationHandler))
        {
            _logger.LogError("Got a free text message with prior conversation, but was unable to match next step {NextStep} to any conversation handler", conversation.NextStep);
            return await _helpCommandHandler.HandleCommandAsync(message, cancellationToken);
        }

        return await conversationHandler.HandleConversationAsync(message, conversation, cancellationToken);
    }
}