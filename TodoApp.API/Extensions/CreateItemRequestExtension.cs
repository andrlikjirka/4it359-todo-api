using TodoApp.Api.Data;
using TodoApp.Api.Messages;

namespace TodoApp.Api.Extensions;

public static class CreateItemRequestExtension
{
    public static Item ToItem(this CreateItemRequest request)
    {
        return new Item
        {
            Title = request.Title,
            Progress = request.Progress,
            DueDate = request.DueDate,
            Priority = request.Priority
        };
    }
}