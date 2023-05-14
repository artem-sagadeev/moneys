using ApplicationServices.Operations;
using ApplicationServices.ShoppingLists;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Operations.Dtos;
using Operations.Entities;
using ShoppingLists.Dtos;
using ShoppingLists.Entities;

namespace Web.Pages.ShoppingLists;

public class DetailsModel : PageModel
{
    private readonly IShoppingListsService _shoppingListsService;
    private readonly IOperationsService _operationsService;

    public DetailsModel(IShoppingListsService shoppingListsService, IOperationsService operationsService)
    {
        _shoppingListsService = shoppingListsService;
        _operationsService = operationsService;
    }

    public ShoppingList ShoppingList { get; private set; }
    
    public List<Card> AllCards { get; private set; }
    
    public async Task<IActionResult> OnGet(Guid id)
    {
        var shoppingList = await _shoppingListsService.GetShoppingListWithItems(User, id);
        var userCards = await _operationsService.GetAllUserCards(User);
        
        ShoppingList = shoppingList;
        ShoppingList.ListItems = shoppingList?.ListItems.OrderBy(item => item.CreationTime).ToList();
        AllCards = userCards?.OrderBy(card => card.Name).ToList();
        
        return Page();
    }

    public async Task<IActionResult> OnPostListItem(CreateListItemDto dto)
    {
        await _shoppingListsService.CreateListItem(User, dto);

        return RedirectToPage(new {id = dto.ShoppingListId});
    }

    public async Task<IActionResult> OnPostPurchase(Guid listId, Guid itemId)
    {
        await _shoppingListsService.PurchaseListItem(User, itemId);

        return RedirectToPage(new {id = listId});
    }
    
    public async Task<IActionResult> OnPostCancelPurchase(Guid listId, Guid itemId)
    {
        await _shoppingListsService.CancelPurchaseListItem(User, itemId);

        return RedirectToPage(new {id = listId});
    }

    public async Task<IActionResult> OnPostToOperations(CreatePaymentDto dto)
    {
        await _operationsService.CreatePayment(User, dto);

        return RedirectToPage(new {id = dto.ListId});
    }

    public async Task<IActionResult> OnPostUpdateListItem(UpdateListItemDto dto)
    {
        await _shoppingListsService.UpdateListItem(User, dto);

        return RedirectToPage(new {id = dto.ListId});
    }

    public async Task<IActionResult> OnPostDeleteListItem(Guid listId, Guid itemId)
    {
        await _shoppingListsService.DeleteListItem(User, itemId);

        return RedirectToPage(new {id = listId});
    }
}