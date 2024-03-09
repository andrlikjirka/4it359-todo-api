using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;
using TodoApp.Api.Helpers;
using TodoApp.Api.Messages;

namespace TodoApp.API.Test;

public class ItemsRepositoryTest
{
    [Fact]
    public async Task List_ReturnsAllItems()
    {
        // arrange
        var items = ItemGenerator.GenerateItems(2).ToArray();
        var context = await CreateContext(items);
        // act
        var sut = new ItemRepository(context);
        var result = await sut.List();
        // assert
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
        var items = Enumerable.Empty<Item>();
        var context = await CreateContext(items);

        var sut = new ItemRepository(context);
        var result = await sut.Find(int.MaxValue);

        Assert.Null(result);
    }

    [Fact]
    public async Task Update_ItemExists_Updates()
    {
        var item = new Item
        {
            Title = "Origina",
            Progress = 0,
            Priority = 0,
            DueDate = DateTime.MinValue
        };
        var context = await CreateContext(new[] { item });

        var sut = new ItemRepository(context);
        var result = await sut.Update(new Item
        {
            Id = item.Id,
            Title = "Updated",
            Progress = 100,
            Priority = 5,
            DueDate = DateTime.MaxValue
        });
        var foundUpdatedItem = await context.Items.FindAsync(item.Id);

        Assert.Equivalent(item, result);
        Assert.Equivalent(item, foundUpdatedItem);
    }

    [Fact]
    public async Task Update_ItemDoesNotExist_ReturnsNull()
    {
        var context = await CreateContext(Enumerable.Empty<Item>());

        var sut = new ItemRepository(context);
        var result = await sut.Update(new Item
        {
            Id = int.MaxValue
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task Add_AddsItem()
    {
        var item = new Item
        {
            Title = "Learn C#",
            Progress = 0,
            Priority = 3,
            DueDate = DateTime.MinValue
        };
        var context = await CreateContext(new[] { item });

        var sut = new ItemRepository(context);
        var result = await sut.Add(item);
        var foundNewItem = await context.Items.FindAsync(item.Id);

        Assert.Equivalent(item, result);
        Assert.Equivalent(item, foundNewItem);
    }

    [Fact]
    public async Task Remove_RemovesItem()
    {
        var item = ItemGenerator.GenerateItems(1).First();
        var context = await CreateContext(new[] { item });

        var sut = new ItemRepository(context);
        var result = await sut.Remove(item);

        Assert.Equivalent(item, result);
        Assert.Null(await context.Items.FindAsync(item.Id));
    }

    [Fact]
    public async Task FindByQuery_MatchingItemsExists_ReturnsMatchingItems()
    {
        var items = ItemGenerator.GenerateItems().ToArray();
        var queryItems = items
            .Where(item => item.Title.ToLower().Contains("Learn".ToLower()))
            .Where(item => item.Progress > 20)
            .Where(item => item.Progress < 80)
            .Where(item => item.DueDate > DateTime.Now)
            .Where(item => item.DueDate < DateTime.MaxValue)
            .ToArray();
        var context = await CreateContext(items);

        var sut = new ItemRepository(context);
        var result = await sut.FindByQuery(new QueryRequest
        {
            Name = "Learn",
            ProgressFrom = 20,
            ProgressTo = 80,
            DueDateFrom = DateTime.Now,
            DueDateTo = DateTime.MaxValue,
            Limit = items.Length
        });

        Assert.Equivalent(queryItems, result);
    }

    [Fact]
    public async Task FindByQuery_MatchingItemsNotExists_ReturnsEmptyArray()
    {
        var items = ItemGenerator.GenerateItems().ToArray();
        var context = await CreateContext(items);

        var sut = new ItemRepository(context);
        var result = await sut.FindByQuery(new QueryRequest
        {
            Name = "Hello World"
        });

        Assert.Equivalent(Array.Empty<Item>(), result);
    }

    [Fact]
    public async Task FindByPriority_ExistItemsWithMatchingPriority_ReturnsItems()
    {
        var items = ItemGenerator.GenerateItems().ToArray();
        var priorityItems = items
            .Where(item => item.Priority == 2);
        var context = await CreateContext(items);

        var sut = new ItemRepository(context);
        var result = await sut.FindByPriority(2,null);
        
        Assert.Equivalent(priorityItems, result);
    }
    
    private async Task<ItemContext> CreateContext(IEnumerable<Item> seed)
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