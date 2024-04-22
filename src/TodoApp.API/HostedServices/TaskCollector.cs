using Microsoft.Extensions.Options;
using TodoApp.Api.Configuration;
using TodoApp.Api.Data;

namespace TodoApp.Api.HostedServices;

/**
 * Background Task that periodically fetches items and removes items with progress 100 
 */
public class TaskCollector : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly TaskCollectorOptions _options;

    public TaskCollector(IServiceScopeFactory scopeFactory, IOptions<TaskCollectorOptions> options)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IItemRepository>();
                var items = await repository.List();

                foreach (var item in items.Where(x =>  x.Priority > _options.MinPriorityThreshold && x.Progress > 99 ))
                {
                    await repository.Remove(item);
                }
            }
            
            await Task.Delay(_options.SweepInterval, stoppingToken);
        }
    }
}