using Microsoft.EntityFrameworkCore;

namespace TodoApp.Api.Data;

public class ItemContext : DbContext
{
    public ItemContext (DbContextOptions<ItemContext> options)
        : base(options) { }

    public DbSet<Item> Items { get; init; }
}
