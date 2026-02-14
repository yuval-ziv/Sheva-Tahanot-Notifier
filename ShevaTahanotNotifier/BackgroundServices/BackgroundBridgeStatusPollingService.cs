using ShevaTahanotNotifier.Services;

namespace ShevaTahanotNotifier.BackgroundServices;

public class BackgroundBridgeStatusPollingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundBridgeStatusPollingService> _logger;

    public BackgroundBridgeStatusPollingService(IServiceProvider serviceProvider, ILogger<BackgroundBridgeStatusPollingService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting bridge status polling service");
        await PollBridgeStatusAsync(stoppingToken);
    }

    private async Task PollBridgeStatusAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogDebug("Polling bridge status");
                using IServiceScope scope = _serviceProvider.CreateScope();
                var bridgeStatusService = scope.ServiceProvider.GetRequiredService<IBridgeStatusService>();

                await bridgeStatusService.GetLastBridgeStatusAsync(true, false, stoppingToken);
                _logger.LogDebug("Bridge status polling finished successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bridge status polling failed");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}