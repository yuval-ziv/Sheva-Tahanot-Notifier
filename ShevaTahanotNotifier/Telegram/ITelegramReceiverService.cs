namespace ShevaTahanotNotifier.Telegram;

public interface ITelegramReceiverService
{
    Task ReceiveAsync(CancellationToken stoppingToken);
}