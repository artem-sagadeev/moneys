using System.Security.Claims;
using ShoppingLists.Dtos;
using ShoppingLists.Entities;

namespace ApplicationServices.ShoppingLists;

public interface IShoppingListsService
{
    public Task<List<ShoppingList>> GetAllUserShoppingListsWithItems(ClaimsPrincipal user);

    public Task CreateShoppingList(ClaimsPrincipal user, CreateShoppingListDto dto);

    public Task UpdateShoppingList(ClaimsPrincipal user, UpdateShoppingListDto dto);

    public Task DeleteShoppingList(ClaimsPrincipal user, Guid shoppingListId);
}