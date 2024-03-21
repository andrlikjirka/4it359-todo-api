using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Api.Data;
using TodoApp.Api.Helpers;
using TodoApp.Api.Messages;

namespace TodoApp.API.IntegrationTest.ControllersTests;

public class ItemsApiTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ItemsApiTest (WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsAllItems()
    {
        using var scope = _factory.Services.CreateScope();
        var items = await scope.ServiceProvider.GetRequiredService<IItemRepository>().List();

        var response = await _client.GetAsync("/api/items");
        response.EnsureSuccessStatusCode();
        var responseItems = Deserialize<Item[]>(await response.Content.ReadAsStringAsync());
        
        Assert.Equal(items.Length, responseItems?.Length);
        Assert.Equivalent(items, responseItems);
    }

    [Fact]
    public async Task Get_ItemExists_ReturnsItem()
    {
        using var scope = _factory.Services.CreateScope();
        var item = (await scope.ServiceProvider.GetRequiredService<IItemRepository>().List()).Last();

        var response = await _client.GetAsync($"/api/items/{item.Id}");
        response.EnsureSuccessStatusCode();
        var responseItem = Deserialize<Item>(await response.Content.ReadAsStringAsync());
        
        Assert.Equivalent(item, responseItem);

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
        var itemResponse = Deserialize<Item>(await response.Content.ReadAsStringAsync()); 
        Assert.Equal(request.Title, itemResponse?.Title);
        Assert.Equal(request.Priority, itemResponse?.Priority);
        Assert.Equal(request.Progress, itemResponse?.Progress);
        Assert.Equal(request.DueDate, itemResponse?.DueDate);
    }

    [Fact]
    public async Task Delete_ItemExists_ReturnsRemovedItem()
    {
        using var scope = _factory.Services.CreateScope();
        var item = (await scope.ServiceProvider.GetRequiredService<IItemRepository>().List()).Last();
        var content = new StringContent(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");

        var response = await _client.DeleteAsync($"api/items/{item.Id}");
        var responseItem = Deserialize<Item>(await response.Content.ReadAsStringAsync());
        Assert.Equivalent(item, responseItem);
    }

    [Fact]
    public async Task Delete_ItemDoesNotExist_ReturnsNotFound()
    {
        var item = ItemGenerator.GenerateItems(1).First();

        var response = await _client.DeleteAsync($"api/Items/{item.Id}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
    }


}