namespace ShevaTahanotNotifier.Database.Entities.NotificationProviderConfiguration;

public class TelegramNotificationProviderConfiguration : BaseNotificationProviderConfiguration
{
    public required long ChatId { get; set; }
}