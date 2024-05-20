using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Messages;

namespace TodoApp.Api.Data;

public class ItemRepository : IItemRepository
{
    private readonly ItemContext _context;

    public ItemRepository(ItemContext context)
    {
        _context = context;
    }

    public Task<Item[]> List()
    {
        return _context.Items.ToArrayAsync();
    }

    public ValueTask<Item> Find(int id)
    {
        return _context.Items.FindAsync(id);
    }

    public async Task<Item> Update(Item item)
    {
        var saved = await Find(item.Id);
        if (saved == null)
        {
            return null;
        }

        saved.Title = item.Title;
        saved.Progress = item.Progress;
        saved.DueDate = item.DueDate;
        saved.Priority = item.Priority;
        
        await _context.SaveChangesAsync();
        return saved;
    }

    public async Task<Item> Add(Item item)
    {
        item.Id = default;
        _context.Items.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<Item> Remove(Item item)
    {
        _context.Items.Remove(item);
        await _context.SaveChangesAsync();
        return item;
    }

    /**
     * Method finds items by the given query
     */
    public async Task<Item[]> FindByQuery(QueryRequest request)
    {
        IQueryable<Item> items = _context.Items;
        if (request.Name is not null)
        {
            items = items.Where(item => item.Title.ToLower().Contains(request.Name.ToLower()));
        }

        if (request.ProgressFrom is not null)
        {
            items = items.Where(item => item.Progress >= request.ProgressFrom);
        }

        if (request.ProgressTo is not null)
        {
            items = items.Where(item => item.Progress <= request.ProgressTo);
        }

        if (request.DueDateFrom is not null)
        {
            items = items.Where(item => item.DueDate >= request.DueDateFrom);
        }

        if (request.DueDateTo is not null)
        {
            items = items.Where(item => item.DueDate <= request.DueDateTo);
        }

        return await items
            .Take(request.Limit)
            .ToArrayAsync();
    }

    /**
     * Method finds items with giver priority and takes only the given limit
     */
    public async Task<Item[]> FindByPriority(int priority, int? limit)
    {
        IQueryable<Item> items = _context.Items;
        items = items.Where(item => item.Priority == priority);
        if (limit is not null)
        {
            items = items.Take(limit.Value);
        }
        return await items.ToArrayAsync();
    }
    
}