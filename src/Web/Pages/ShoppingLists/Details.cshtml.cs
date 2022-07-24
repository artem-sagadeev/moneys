using ApplicationServices.ShoppingLists;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingLists.Dtos;
using ShoppingLists.Entities;

namespace Web.Pages.ShoppingLists;

public class DetailsModel : PageModel
{
    private readonly IShoppingListsService _shoppingListsService;

    public DetailsModel(IShoppingListsService shoppingListsService)
    {
        _shoppingListsService = shoppingListsService;
    }
    
    public ShoppingList ShoppingList { get; private set; }
    
    public async Task<IActionResult> OnGet(Guid id)
    {
        var shoppingList = await _shoppingListsService.GetShoppingListWithItems(User, id);

        ShoppingList = shoppingList;
        ShoppingList.ListItems = shoppingList.ListItems.OrderBy(item => item.CreationTime).ToList();
        
        return Page();
    }

    public async Task<IActionResult> OnPostListItem(CreateListItemDto dto)
    {
        await _shoppingListsService.CreateListItem(User, dto);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostPurchase(Guid id)
    {
        await _shoppingListsService.PurchaseListItem(User, id);

        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostCancelPurchase(Guid id)
    {
        await _shoppingListsService.CancelPurchaseListItem(User, id);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateListItem(UpdateListItemDto dto)
    {
        await _shoppingListsService.UpdateListItem(User, dto);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteListItem(Guid id)
    {
        await _shoppingListsService.DeleteListItem(User, id);

        return RedirectToPage();
    }
}