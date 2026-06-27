namespace Invento.Gateaway;

public class ConsulWatcherService : BackgroundService
{
    private readonly ConsulConfigurationProvider _provider;
    public ConsulWatcherService(ConsulConfigurationProvider provider) => _provider = provider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _provider.LoadAsync();     
            await Task.Delay(10000, stoppingToken);    
        }
    }
}