using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using TodoApp.Api.Data;
using TodoApp.Api.Helpers;
using TodoApp.Api.HostedServices;

namespace TodoApp.API.Test.HostedServicesTests;

public class TaskCollectorTest
{
    [Fact]
    public async Task StartAndStopBackgroundService()
    {
        var items = ItemGenerator.GenerateItems().ToArray();
        var itemRepositoryMock = Substitute.For<IItemRepository>();
        itemRepositoryMock.List().Returns(items);
        
        var serviceProviderMock = Substitute.For<IServiceProvider>();
        serviceProviderMock.GetService(typeof(IItemRepository)).Returns(itemRepositoryMock);

        using var scopeMock = Substitute.For<IServiceScope>();
        scopeMock.ServiceProvider.Returns(serviceProviderMock);

        var scopeFactoryMock = Substitute.For<IServiceScopeFactory>();
        scopeFactoryMock.CreateScope().Returns(scopeMock);

        var sut = new TaskCollector(scopeFactoryMock);
        var cancellationToken = new CancellationToken();
        await sut.StartAsync(cancellationToken);
        Assert.False(sut.ExecuteTask.IsCompleted);
        await Task.Delay(2000);
        await sut.StopAsync(cancellationToken);
        Assert.True(sut.ExecuteTask.IsCompleted);
    }
}