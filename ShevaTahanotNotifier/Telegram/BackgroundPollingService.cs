namespace ShevaTahanotNotifier.Telegram;

public class BackgroundPollingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundPollingService> _logger;

    public BackgroundPollingService(IServiceProvider serviceProvider, ILogger<BackgroundPollingService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting polling service");
        await PollTelegramMessagesAsync(stoppingToken);
    }

    private async Task PollTelegramMessagesAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                var telegramReceiverService = scope.ServiceProvider.GetRequiredService<ITelegramReceiverService>();

                await telegramReceiverService.ReceiveAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Polling failed");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}