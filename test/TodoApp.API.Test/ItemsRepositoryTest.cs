using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;
using TodoApp.Api.Helpers;

namespace TodoApp.API.Test;

public class ItemsRepositoryTest
{
    [Fact]
    public async Task List_ReturnsAllItems()
    {
        var items = ItemGenerator.GenerateItems(2).ToArray();
        var context = await CreateContext(items);

        var sut = new ItemRepository(context);
        var result = await sut.List();
        
        Assert.Equivalent(items, result);
    }
    
    [Fact]
    public async Task Find_ItemExists_ReturnsItem()
    {
        var items = ItemGenerator.GenerateItems(2).ToArray();
        var context = await CreateContext(items);
        var item = items.First();

        var sut = new ItemRepository(context);
        var result = await sut.Find(item.Id);

        Assert.Equivalent(item, result);
    }
    
    [Fact]
    public async Task Find_ItemDoesNotExist_ReturnsNull()
    {
        var context = await CreateContext(Enumerable.Empty<Item>());

        var sut = new ItemRepository(context);
        var result = await sut.Find(int.MaxValue);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task Update_ItemExists_Updates()
    {
        var item = new Item
        {
            Title = "Original",
            Progress = 0,
            DueDate = DateTime.MinValue,
            Priority = 0
        };

        var context = await CreateContext(new[] { item });

        var sut = new ItemRepository(context);
        var result = await sut.Update(new Item
        {
            Id = item.Id,
            Title = "Updated",
            Progress = 100,
            DueDate = DateTime.MaxValue,
            Priority = 5
        });

        Assert.Equal(item.Id, result.Id);
        Assert.Equal("Updated", result.Title);
        Assert.Equal(100, result.Progress);
        Assert.Equal(DateTime.MaxValue, result.DueDate);
        Assert.Equal(5, result.Priority);
        Assert.Equal(result, await context.Items.FindAsync(result.Id));
    }

    [Fact]
    public async Task Update_ItemDoesNotExist_ReturnsNull()
    {
        var context = await CreateContext(Enumerable.Empty<Item>());

        var sut = new ItemRepository(context);
        var result = await sut.Update(new Item { Id = int.MaxValue });

        Assert.Null(result);
    }
    
    [Fact]
    public async Task Add_AddsItem()
    {
        var item = new Item
        {
            Title = "Learn C#",
            Progress = 6,
            DueDate = DateTime.MaxValue,
            Priority = 3
        };

        var context = await CreateContext(Enumerable.Empty<Item>());
        
        var sut = new ItemRepository(context);
        var result = await sut.Add(item);

        Assert.NotEqual(0, result.Id);
        Assert.Equal("Learn C#", result.Title);
        Assert.Equal(6, result.Progress);
        Assert.Equal(DateTime.MaxValue, result.DueDate);
        Assert.Equal(3, result.Priority);
        Assert.Equal(result, await context.Items.FindAsync(result.Id));
    }
    
    [Fact]
    public async Task Remove_RemovesItem()
    {
        var item = ItemGenerator.GenerateItems(1).First();
        var context = await CreateContext(new[]{ item });
        
        var sut = new ItemRepository(context);
        var result = await sut.Remove(item);
        
        Assert.Equal(item, result);
        Assert.Null(await context.Items.FindAsync(item.Id));
    }
    
    private static async Task<ItemContext> CreateContext(IEnumerable<Item> seed)
    {
        var options = new DbContextOptionsBuilder<ItemContext>()
            .UseInMemoryDatabase(databaseName: nameof(List_ReturnsAllItems))
            .Options;
        var context = new ItemContext(options);
        context.Items.AddRange(seed);
        await context.SaveChangesAsync();

        return context;
    }
}