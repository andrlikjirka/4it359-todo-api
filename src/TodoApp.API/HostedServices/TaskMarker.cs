using TodoApp.Api.Data;

namespace TodoApp.Api.HostedServices;

/**
 * Background Task that periodically fetches items and:
 * mark all past due items as priority 1
 * mark all due today items as priority 2
 * mark all due tomorrow items as priority 3
 */
public class TaskMarker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TaskMarker(IServiceScopeFactory scopeFactory)
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

                foreach (var item in items.Where(x => x.Progress <= 99))
                {
                    if (item.DueDate.Date < DateTime.Today.Date)
                    {
                        item.Priority = 1;
                        await repository.Update(item);
                    }
                    else if (item.DueDate.Date == DateTime.Today.Date)
                    {
                        item.Priority = 2;
                        await repository.Update(item);
                    }
                    else if (item.DueDate.Date == DateTime.Today.AddDays(1).Date)
                    {
                        item.Priority = 3;
                        await repository.Update(item);           
                    }
                }
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}