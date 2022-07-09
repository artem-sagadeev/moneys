using System.Security.Claims;
using Common.Exceptions;
using Common.Extensions;
using Identity.Entities;
using Microsoft.AspNetCore.Identity;
using ShoppingLists.Dtos;
using ShoppingLists.Entities;
using ShoppingLists.Logic.ShoppingLists;

namespace ApplicationServices.ShoppingLists;

public class ShoppingListsService : IShoppingListsService
{
    private readonly UserManager<User> _userManager;
    private readonly IShoppingListService _shoppingListService;

    public ShoppingListsService(UserManager<User> userManager, IShoppingListService shoppingListService)
    {
        _userManager = userManager;
        _shoppingListService = shoppingListService;
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

    private async Task<bool> HasUserAccessToShoppingList(ClaimsPrincipal user, Guid shoppingListId)
    {
        var currentUser = await _userManager.GetUserAsync(user);
        var userShoppingLists = await _shoppingListService.GetByUserId(currentUser.Id);

        return userShoppingLists.Ids().Contains(shoppingListId);
    }
}