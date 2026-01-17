namespace ShevaTahanotNotifier.Configuration;

public class TelegramBotOptions
{
    public const string ConfigurationSectionName = "TelegramBot";

    public required string BotToken { get; init; }
}