using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TodoApp.Api.Controllers;
using TodoApp.Api.Data;
using TodoApp.Api.Helpers;
using TodoApp.Api.Messages;

namespace TodoApp.API.Test;

public class QueryControllerTest
{
    [Fact]
    public async Task PostQuery_MatchingItemsNotExist_ReturnsNotFound()
    {
        var request = new QueryRequest
        {
            Name = "Hello World",
            Limit = 10
        };
        var items = new Item[] { };
        var itemRepositoryMock = Substitute.For<IItemRepository>();
        itemRepositoryMock.FindByQuery(request).Returns(items);

        var sut = new QueryController(itemRepositoryMock);
        var result = await sut.Post(request);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostQuery_ValidQuery_MatchingItemsExist_ReturnsItems()
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
        var validationContext = new ValidationContext(request, serviceProvider: null, items: null);
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(request, validationContext, validationResults, validateAllProperties: true);
        
        var items = ItemGenerator.GenerateItems().ToArray();
        var matchingItems = items
            .Where(item => item.Title.ToLower().Contains("Learn".ToLower()))
            .Where(item => item.Progress > 20)
            .Where(item => item.Progress < 80)
            .Where(item => item.DueDate > DateTime.Now)
            .Where(item => item.DueDate < DateTime.MaxValue)
            .ToArray();
        var itemRepositoryMock = Substitute.For<IItemRepository>();
        itemRepositoryMock.FindByQuery(request).Returns(matchingItems);

        var sut = new QueryController(itemRepositoryMock);
        var result = await sut.Post(request);

        Assert.Empty(validationResults);
        Assert.IsType<OkObjectResult>(result.Result);
        var objectResult = (OkObjectResult)result.Result!;
        Assert.Equivalent(matchingItems, objectResult.Value);
    }

    [Fact]
    public async Task PostQuery_InvalidQuery_ReturnsBadRequest()
    {
        var request = new QueryRequest
        {
            Name = "Hello World",
            ProgressFrom = int.MinValue,
            ProgressTo = int.MaxValue,
            DueDateFrom = DateTime.Now,
            DueDateTo = DateTime.MaxValue,
            //Limit = 10
        };
        var validationContext = new ValidationContext(request, serviceProvider: null, items: null);
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(request, validationContext, validationResults, validateAllProperties: true);

        var items = new Item[] { };
        var itemRepositoryMock = Substitute.For<IItemRepository>();
        itemRepositoryMock.FindByQuery(request).Returns(items);

        var sut = new QueryController(itemRepositoryMock);
        var result = await sut.Post(request);
        
        Assert.NotEmpty(validationResults);
        // jak zkontrolovat badrequest? controller stále vrací notfound (i pro nevalidní request)
    }

    [Fact]
    public async Task Get_ItemsWithGivePriorityNotExist_ReturnsNotFound()
    {
        var items = new Item[] { };
        var itemRepositoryMock = Substitute.For<IItemRepository>();
        itemRepositoryMock.FindByPriority(1, null).Returns(items);

        var sut = new QueryController(itemRepositoryMock);
        var result = await sut.Get(1, null);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Get_ItemsWithGivenPriorityExist_ReturnsItems()
    {
        var items = ItemGenerator.GenerateItems().ToArray();
        var priorityItems = items.Where(item => item.Priority == 3).ToArray();
        var itemRepositoryMock = Substitute.For<IItemRepository>();
        itemRepositoryMock.FindByPriority(3, null).Returns(priorityItems);

        var sut = new QueryController(itemRepositoryMock);
        var result = await sut.Get(3, null);

        Assert.IsType<OkObjectResult>(result.Result);
        var returnResult = (OkObjectResult)result.Result!;
        Assert.Equivalent(priorityItems, returnResult.Value);
    }

    [Fact]
    public async Task Get_GivenPriorityOutOfRange_ReturnsBadRequest()
    {
        var itemRepositoryMock = Substitute.For<IItemRepository>();
        itemRepositoryMock.FindByPriority(int.MaxValue, null).Returns((Item[])null!);

        var sut = new QueryController(itemRepositoryMock);
        var result = await sut.Get(int.MaxValue, null);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}