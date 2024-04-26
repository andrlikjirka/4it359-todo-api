using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Api.Data;

namespace TodoApp.API.IntegrationTest.HostedServicesTests;

public class TaskCollectorTest: IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TaskCollectorTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task PeriodicallyRemovesItemsWithProgress100()
    {
        using var scope = _factory.Services.CreateScope();

        await Task.Delay(5000);
        
        var items = await scope.ServiceProvider.GetRequiredService<IItemRepository>().List();
        foreach (var item in items)
        {
            Assert.False(item.Progress > 99); //check if there is no item with progress 100 (all of them should be removed)
        }

    }
    
}