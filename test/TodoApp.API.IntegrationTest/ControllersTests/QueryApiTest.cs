using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Api.Data;
using TodoApp.Api.Messages;

namespace TodoApp.API.IntegrationTest.ControllersTests;

public class QueryApiTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public QueryApiTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_MatchingItemsNotExist_ReturnsNotFound()
    {
        var request = new QueryRequest
        {
            Name = "Hello World",
            Limit = 10
        };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/query", content);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_MatchingItemsExist_ReturnsItems()
    {
        var request = new QueryRequest
        {
            Name = "Learn",
            ProgressFrom = 20,
            ProgressTo = 80,
            DueDateFrom = DateTime.Now,
            DueDateTo = DateTime.MaxValue,
            Limit = 10
        };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        using var scope = _factory.Services.CreateScope();
        var items = await scope.ServiceProvider.GetRequiredService<IItemRepository>().List();
        var queryMatchingItems = items
            .Where(item => item.Title.ToLower().Contains("Learn".ToLower()))
            .Where(item => item.Progress > 20)
            .Where(item => item.Progress < 80)
            .Where(item => item.DueDate > DateTime.Now)
            .Where(item => item.DueDate < DateTime.MaxValue)
            .ToArray();

        var response = await _client.PostAsync("/api/query", content);
        var responseItems = Deserialize<Item[]>(await response.Content.ReadAsStringAsync());
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(queryMatchingItems, responseItems);
    }

    [Fact]
    public async Task Get_ItemsWithGivenPriorityNotExist_ReturnsItems()
    {
        using var scope = _factory.Services.CreateScope();
        var items = await scope.ServiceProvider.GetRequiredService<IItemRepository>().List();
        var itemsWithGivenPriority = items
            .Where(item => item.Priority == 2)
            .ToArray();
        var response = await _client.GetAsync("api/query/priority/2");
        var responseItems = Deserialize<Item[]>(await response.Content.ReadAsStringAsync());
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(itemsWithGivenPriority, responseItems);
    }

    [Fact]
    public async Task Get_GivenPriorityOutOfRange_ReturnsBadRequest()
    {
        var response = await _client.GetAsync($"api/query/priority/{int.MaxValue}");
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    private T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
    }
}