using Common.Exceptions;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using ShoppingLists.Data;
using ShoppingLists.Dtos;
using ShoppingLists.Entities;

namespace ShoppingLists.Logic.ShoppingLists;

public class ShoppingListService : IShoppingListsService
{
    private readonly ShoppingListsContext _context;

    public ShoppingListService(ShoppingListsContext context)
    {
        _context = context;
    }

    public async Task<List<ShoppingList>> GetByUserId(string userId)
    {
        var shoppingLists = await _context.ShoppingLists.GetByUserId(userId);

        return shoppingLists;
    }

    public async Task<ShoppingList> GetWithItems(Guid id)
    {
        var shoppingList = await _context
            .ShoppingLists
            .Include(list => list.ListItems)
            .SingleOrDefaultAsync();

        if (shoppingList is null)
            throw new EntityNotFoundException();

        return shoppingList;
    }

    public async Task Create(CreateShoppingListDto dto)
    {
        var shoppingList = new ShoppingList(dto);
        _context.ShoppingLists.Add(shoppingList);
        await _context.SaveChangesAsync();
    }

    public async Task Update(UpdateShoppingListDto dto)
    {
        var shoppingList = await _context.ShoppingLists.GetById(dto.Id);

        shoppingList.Name = dto.Name;
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var shoppingList = await _context
            .ShoppingLists
            .Include(list => list.ListItems)
            .SingleOrDefaultAsync();
        
        if (shoppingList is null)
            throw new EntityNotFoundException();
        
        _context.ListItems.RemoveRange(shoppingList.ListItems);
        _context.ShoppingLists.Remove(shoppingList);
        await _context.SaveChangesAsync();
    }
}