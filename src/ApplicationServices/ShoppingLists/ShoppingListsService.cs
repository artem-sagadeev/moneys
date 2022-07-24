using System.Security.Claims;
using Common.Exceptions;
using Common.Extensions;
using Identity.Entities;
using Microsoft.AspNetCore.Identity;
using ShoppingLists.Dtos;
using ShoppingLists.Entities;
using ShoppingLists.Logic.ListItems;
using ShoppingLists.Logic.ShoppingLists;

namespace ApplicationServices.ShoppingLists;

public class ShoppingListsService : IShoppingListsService
{
    private readonly UserManager<User> _userManager;
    private readonly IShoppingListService _shoppingListService;
    private readonly IListItemService _listItemService;

    public ShoppingListsService(UserManager<User> userManager, IShoppingListService shoppingListService, IListItemService listItemService)
    {
        _userManager = userManager;
        _shoppingListService = shoppingListService;
        _listItemService = listItemService;
    }

    public async Task<ShoppingList> GetShoppingListWithItems(ClaimsPrincipal user, Guid shoppingListId)
    {
        var currentUser = await _userManager.GetUserAsync(user);
        var shoppingList = await _shoppingListService.GetWithItems(shoppingListId);
        
        if (shoppingList.UserId != currentUser.Id)
            throw new NoAccessException();

        return shoppingList;
    }

    public async Task<List<ShoppingList>> GetAllUserShoppingListsWithItems(ClaimsPrincipal user)
    {
        var currentUser = await _userManager.GetUserAsync(user);
        var shoppingLists = await _shoppingListService.GetByUserIdWithItems(currentUser.Id);

        return shoppingLists;
    }

    public async Task CreateShoppingList(ClaimsPrincipal user, CreateShoppingListDto dto)
    {
        var currentUser = await _userManager.GetUserAsync(user);
        dto.UserId = currentUser.Id;

        await _shoppingListService.Create(dto);
    }

    public async Task UpdateShoppingList(ClaimsPrincipal user, UpdateShoppingListDto dto)
    {
        if (!await HasUserAccessToShoppingList(user, dto.Id))
            throw new NoAccessException();

        await _shoppingListService.Update(dto);
    }

    public async Task DeleteShoppingList(ClaimsPrincipal user, Guid shoppingListId)
    {
        if (!await HasUserAccessToShoppingList(user, shoppingListId))
            throw new NoAccessException();

        await _shoppingListService.Delete(shoppingListId);
    }

    public async Task CreateListItem(ClaimsPrincipal user, CreateListItemDto dto)
    {
        if (!await HasUserAccessToShoppingList(user, dto.ShoppingListId))
            throw new NoAccessException();

        await _listItemService.Create(dto);
    }
    
    public async Task UpdateListItem(ClaimsPrincipal user, UpdateListItemDto dto)
    {
        if (!await HasUserAccessToListItem(user, dto.Id))
            throw new NoAccessException();

        await _listItemService.Update(dto);
    }

    public async Task DeleteListItem(ClaimsPrincipal user, Guid listItemId)
    {
        if (!await HasUserAccessToListItem(user, listItemId))
            throw new NoAccessException();

        await _listItemService.Delete(listItemId);
    }

    public async Task PurchaseListItem(ClaimsPrincipal user, Guid listItemId)
    {
        if (!await HasUserAccessToListItem(user, listItemId))
            throw new NoAccessException();

        await _listItemService.Purchase(listItemId);
    }

    public async Task CancelPurchaseListItem(ClaimsPrincipal user, Guid listItemId)
    {
        if (!await HasUserAccessToListItem(user, listItemId))
            throw new NoAccessException();

        await _listItemService.CancelPurchase(listItemId);
    }

    private async Task<bool> HasUserAccessToShoppingList(ClaimsPrincipal user, Guid shoppingListId)
    {
        var currentUser = await _userManager.GetUserAsync(user);
        var userShoppingLists = await _shoppingListService.GetByUserId(currentUser.Id);

        return userShoppingLists.Ids().Contains(shoppingListId);
    }
    
    private async Task<bool> HasUserAccessToListItem(ClaimsPrincipal user, Guid listItemId)
    {
        var currentUser = await _userManager.GetUserAsync(user);
        var userShoppingLists = await _shoppingListService.GetByUserId(currentUser.Id);
        var listItem = await _listItemService.GetById(listItemId);

        return userShoppingLists.Ids().Contains(listItem.ShoppingListId);
    }
}