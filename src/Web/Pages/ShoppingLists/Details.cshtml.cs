using ApplicationServices.Operations;
using ApplicationServices.ShoppingLists;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Operations.Dtos;
using Operations.Dtos.PaymentRecords;
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

    public async Task<IActionResult> OnPostToOperations(CreatePaymentRecordDto recordDto)
    {
        await _operationsService.CreatePayment(User, recordDto);

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