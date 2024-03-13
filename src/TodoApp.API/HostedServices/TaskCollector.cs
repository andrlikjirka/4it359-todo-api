using TodoApp.Api.Data;

namespace TodoApp.Api.HostedServices;

public class TaskCollector : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TaskCollector(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IItemRepository>();
                var items = await repository.List();

                foreach (var item in items.Where(x => x.Progress > 99))
                {
                    await repository.Remove(item);
                }
            }
            
            await Task.Delay(5000, stoppingToken);
        }
    }
}