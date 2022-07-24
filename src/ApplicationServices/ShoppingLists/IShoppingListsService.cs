using System.Security.Claims;
using ShoppingLists.Dtos;
using ShoppingLists.Entities;

namespace ApplicationServices.ShoppingLists;

public interface IShoppingListsService
{
    public Task<ShoppingList> GetShoppingListWithItems(ClaimsPrincipal user, Guid shoppingListId);

    public Task<List<ShoppingList>> GetAllUserShoppingListsWithItems(ClaimsPrincipal user);

    public Task CreateShoppingList(ClaimsPrincipal user, CreateShoppingListDto dto);

    public Task UpdateShoppingList(ClaimsPrincipal user, UpdateShoppingListDto dto);

    public Task DeleteShoppingList(ClaimsPrincipal user, Guid shoppingListId);

    public Task CreateListItem(ClaimsPrincipal user, CreateListItemDto dto);

    public Task UpdateListItem(ClaimsPrincipal user, UpdateListItemDto dto);

    public Task DeleteListItem(ClaimsPrincipal user, Guid listItemId);

    public Task PurchaseListItem(ClaimsPrincipal user, Guid listItemId);
    
    public Task CancelPurchaseListItem(ClaimsPrincipal user, Guid listItemId);
}