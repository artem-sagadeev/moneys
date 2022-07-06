using Microsoft.EntityFrameworkCore;
using ShoppingLists.Entities;

namespace ShoppingLists.Data;

public class ShoppingListsContext : DbContext
{
    public DbSet<ShoppingList> ShoppingLists { get; set; }
    
    public DbSet<ListItem> ListItems { get; set; }

    public ShoppingListsContext(DbContextOptions<ShoppingListsContext> options) : base(options) {}
}