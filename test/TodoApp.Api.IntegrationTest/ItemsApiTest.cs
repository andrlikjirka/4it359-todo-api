using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Api.Data;
using TodoApp.Api.Helpers;
using TodoApp.Api.Messages;

namespace TodoApp.API.IntegrationTest;

public class ItemsApiTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    
    public ItemsApiTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsAllItems()
    {
        using var scope = _factory.Services.CreateScope();
        var itemsCount = (await scope.ServiceProvider.GetRequiredService<IItemRepository>().List()).Length;
        
        var response = await _client.GetAsync("/api/items");
        response.EnsureSuccessStatusCode();
        var items = Deserialize<Item[]>(await response.Content.ReadAsStringAsync());
        
        Assert.Equal(itemsCount, items?.Length);
    }
    
    [Fact]
    public async Task Get_ItemExists_ReturnsItem()
    {
        using var scope = _factory.Services.CreateScope();
        var lastItem = (await scope.ServiceProvider.GetRequiredService<IItemRepository>().List()).Last();
        
        var response = await _client.GetAsync($"/api/items/{lastItem.Id}");
        response.EnsureSuccessStatusCode();
        var item = Deserialize<Item>(await response.Content.ReadAsStringAsync());
        
        Assert.Equal(lastItem.Id, item?.Id);
        Assert.Equal(lastItem.Priority, item?.Priority);
        Assert.Equal(lastItem.DueDate, item?.DueDate);
        Assert.Equal(lastItem.Progress, item?.Progress);
        Assert.Equal(lastItem.Title, item?.Title);
    }
    
    [Fact]
    public async Task Get_ItemDoesNotExist_Returns404()
    {
        var response = await _client.GetAsync($"/api/items/{int.MaxValue}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Put_ItemExists_ReturnsNoContent()
    {
        using var scope = _factory.Services.CreateScope();
        var item = (await scope.ServiceProvider.GetRequiredService<IItemRepository>().List()).Last();
        item.Title = "Updated";
        var content = new StringContent(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/api/items", content);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task Put_ItemDoesNotExist_ReturnsNotFound()
    {
        var item = ItemGenerator.GenerateItems(1).First();
        var content = new StringContent(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/api/items", content);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Post_ReturnsCreatedItem()
    {
        var request = new CreateItemRequest
        {
            Title = "Find a bug",
            Priority = 1,
            Progress = 50,
            DueDate = DateTime.UnixEpoch
        };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/items", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var item = Deserialize<Item>(await response.Content.ReadAsStringAsync());
        
        Assert.Equal(request.Title, item?.Title);
        Assert.Equal(request.Priority, item?.Priority);
        Assert.Equal(request.Progress, item?.Progress);
        Assert.Equal(request.DueDate, item?.DueDate);
    }
    
    [Fact]
    public async Task Delete_ItemExists_ReturnsRemovedItem()
    {
        using var scope = _factory.Services.CreateScope();
        var lastItem = (await scope.ServiceProvider.GetRequiredService<IItemRepository>().List()).Last();
        var item = new Item
        {
            Id = lastItem.Id,
            Title = "Fix a bug",
            Priority = 2,
            Progress = 1,
            DueDate = DateTime.UnixEpoch
        };

        var content = new StringContent(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/api/items", content);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task Delete_ItemDoesNotExist_ReturnsNotFound()
    {
        var item = ItemGenerator.GenerateItems(1).First();

        var response = await _client.DeleteAsync($"/api/items/{item.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Post_BodyTooLong_Returns413()
    {
        var request = new CreateItemRequest
        {
            Title = GenerateLongBody(),
            Priority = 1,
            Progress = 50,
            DueDate = DateTime.Now
        };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/items", content);

        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, response.StatusCode);
    }

    private static string GenerateLongBody()
    {
        const string baseString = "Lorem Ipsum";
        StringBuilder sb = new StringBuilder();

        while (sb.Length < 500)
        {
            sb.Append(baseString);
        }

        return sb.ToString();
    }
    
    private T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions(){PropertyNameCaseInsensitive = true});
    }
}