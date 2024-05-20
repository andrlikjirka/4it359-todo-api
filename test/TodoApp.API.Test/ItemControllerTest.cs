using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TodoApp.Api.Controllers;
using TodoApp.Api.Data;
using TodoApp.Api.Helpers;
using TodoApp.Api.Messages;

namespace TodoApp.API.Test;

public class ItemControllerTest
{
    [Fact]
    public async Task Get_ReturnsAllItems()
    {
        var items = ItemGenerator.GenerateItems(2).ToArray();
        var itemRepositoryMock = Substitute.For<IItemRepository>();
        itemRepositoryMock.List().Returns(items);

        var sut = new ItemsController(itemRepositoryMock);
        var result = await sut.Get();
        
        Assert.Equivalent(items, result.Value);
    }
    
    [Fact]
    public async Task Get_ItemExists_ReturnsItem()
    {
        var item = ItemGenerator.GenerateItems(2).First();
        var itemRepositoryMock = Substitute.For<IItemRepository>();
        itemRepositoryMock.Find(item.Id).Returns(item);

        var sut = new ItemsController(itemRepositoryMock);
        var result = await sut.Get(item.Id);
        
        Assert.Equivalent(item, result.Value);
    }

    [Fact]
    public async Task Get_ItemDoesNotExist_ReturnsNotFound()
    {
        var itemRepositoryMock = Substitute.For<IItemRepository>();
        itemRepositoryMock.Find(int.MaxValue).Returns((Item)null!);

        var sut = new ItemsController(itemRepositoryMock);
        var result = await sut.Get(int.MaxValue);
        
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Put_ItemExists_ReturnsNoContent()
    {
        var item = ItemGenerator.GenerateItems(1).First();
        var itemRepositoryMock = Substitute.For<IItemRepository>();
        itemRepositoryMock.Update(item).Returns(item);

        var sut = new ItemsController(itemRepositoryMock);
        var result = await sut.Put(item);
        
        Assert.IsType<NoContentResult>(result);
        await itemRepositoryMock.Received().Update(item);
    }

    [Fact]
    public async Task Put_ItemDoesNotExist_ReturnsNotFound()
    {
        var item = ItemGenerator.GenerateItems(1).First();
        var itemRepositoryMock = Substitute.For<IItemRepository>();
        itemRepositoryMock.Update(item).Returns((Item)null!);

        var sut = new ItemsController(itemRepositoryMock);
        var result = await sut.Put(item);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Post_ReturnsCreatedItem()
    {
        var item = ItemGenerator.GenerateItems(1).First();
        var itemRepositoryMock = Substitute.For<IItemRepository>();
        itemRepositoryMock.Add(Arg.Is<Item>(x => 
                x.Title == item.Title
                && x.Priority == item.Priority 
                && x.Progress == item.Progress 
                && x.DueDate == item.DueDate))
            .Returns(item);
        
        var sut = new ItemsController(itemRepositoryMock);
        var result = await sut.Post(new CreateItemRequest
        {
            Title = item.Title,
            Priority = item.Priority,
            Progress = item.Progress,
            DueDate = item.DueDate
        });
        
        Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdResult = (CreatedAtActionResult)result.Result;
        Assert.Equal(item, createdResult.Value);
    }

    [Fact]
    public async Task Delete_ItemExists_ReturnsRemovedItem()
    {
        var item = ItemGenerator.GenerateItems(1).First();
        var itemRepositoryMock = Substitute.For<IItemRepository>();
        itemRepositoryMock.Find(item.Id).Returns(item);
        itemRepositoryMock.Remove(item).Returns(item);

        var sut = new ItemsController(itemRepositoryMock);
        var result = await sut.Delete(item.Id);

        Assert.Equivalent(item, result.Value);
    }

    [Fact]
    public async Task Delete_ItemDoesNotExist_ReturnsNotFound()
    {
        var item = ItemGenerator.GenerateItems(1).First();
        var itemRepositoryMock = Substitute.For<IItemRepository>();
        itemRepositoryMock.Find(item.Id).Returns((Item)null!);

        var sut = new ItemsController(itemRepositoryMock);
        var result = await sut.Delete(item.Id);
        
        Assert.IsType<NotFoundResult>(result.Result);
    }
    
}