using ShevaTahanotNotifier.Telegram;

namespace ShevaTahanotNotifier.BackgroundServices;

public class BackgroundTelegramPollingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundTelegramPollingService> _logger;

    public BackgroundTelegramPollingService(IServiceProvider serviceProvider, ILogger<BackgroundTelegramPollingService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting telegram polling service");
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
                _logger.LogError(ex, "Telegram polling failed");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}