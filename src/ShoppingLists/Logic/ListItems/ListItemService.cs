using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using ShoppingLists.Data;
using ShoppingLists.Dtos;
using ShoppingLists.Entities;

namespace ShoppingLists.Logic.ListItems;

public class ListItemService : IListItemService
{
    private readonly ShoppingListsContext _context;

    public ListItemService(ShoppingListsContext context)
    {
        _context = context;
    }

    public async Task<ListItem> GetById(Guid id)
    {
        var listItem = await _context.ListItems.GetById(id);

        return listItem;
    }

    public async Task<List<ListItem>> GetByShoppingListId(Guid shoppingListId)
    {
        var listItems = await _context.ListItems.Where(item => item.ShoppingListId == shoppingListId).ToListAsync();

        return listItems;
    }

    public async Task Create(CreateListItemDto dto)
    {
        var listItem = new ListItem(dto);
        
        _context.ListItems.Add(listItem);
        await _context.SaveChangesAsync();
    }

    public async Task Update(UpdateListItemDto dto)
    {
        var listItem = await _context.ListItems.GetById(dto.Id);

        listItem.Name = dto.Name;
        listItem.Price = dto.Price;
        listItem.Count = dto.Count;
        await _context.SaveChangesAsync();
    }

    public async Task Purchase(Guid id)
    {
        var listItem = await _context.ListItems.GetById(id);

        listItem.IsPurchased = true;
        await _context.SaveChangesAsync();
    }

    public async Task CancelPurchase(Guid id)
    {
        var listItem = await _context.ListItems.GetById(id);

        listItem.IsPurchased = false;
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var listItem = await _context.ListItems.GetById(id);

        _context.Remove(listItem);
        await _context.SaveChangesAsync();
    }
}