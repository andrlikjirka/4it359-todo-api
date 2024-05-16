using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Api.Data;

namespace TodoApp.API.IntegrationTest.HostedServicesTests;

public class TaskMarkerTest: IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TaskMarkerTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task MarksAllPastDueItemsAsPriority1()
    {
        using var scope = _factory.Services.CreateScope();

        await Task.Delay(5000);

        var items = await scope.ServiceProvider.GetRequiredService<IItemRepository>().List();
        foreach (var item in items.Where(x => x.Progress <= 99))
        {
            if (item.DueDate.Date < DateTime.Today.Date)
            {
                Assert.Equal(1, item.Priority);
            }
        }
    }

    [Fact]
    public async Task MarksAllDueTodayItemsAsPriority2()
    {
        using var scope = _factory.Services.CreateScope();

        await Task.Delay(5000);

        var items = await scope.ServiceProvider.GetRequiredService<IItemRepository>().List();
        foreach (var item in items.Where(x => x.Progress <= 99))
        {
            if (item.DueDate.Date == DateTime.Today.Date)
            {
                Assert.Equal(2, item.Priority);
            }
        }
    }

    [Fact]
    public async Task MarksAllDueTomorrowItemsAsPriority3()
    {
        using var scope = _factory.Services.CreateScope();

        await Task.Delay(5000);

        var items = await scope.ServiceProvider.GetRequiredService<IItemRepository>().List();
        foreach (var item in items.Where(x => x.Progress <= 99))
        {
            if (item.DueDate.Date == DateTime.Today.AddDays(1).Date)
            {
                Assert.Equal(3, item.Priority);
            }
        }
    }
    
}