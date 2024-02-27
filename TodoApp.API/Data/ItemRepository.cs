using Microsoft.EntityFrameworkCore;

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
}