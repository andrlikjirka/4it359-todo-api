using TodoApp.Api.Messages;

namespace TodoApp.Api.Data;

public interface IItemRepository
{
    Task<Item[]> List();
    ValueTask<Item> Find(int id);
    Task<Item> Update(Item item);
    Task<Item> Add(Item item);
    Task<Item> Remove(Item item);
    Task<Item[]> FindByQuery(QueryRequest request);
    Task<Item[]> FindByPriority(int priority, int? limit);
}