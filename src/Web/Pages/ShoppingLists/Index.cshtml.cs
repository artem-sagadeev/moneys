using ApplicationServices.Operations;
using ApplicationServices.ShoppingLists;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Operations.Dtos;
using Operations.Entities;
using ShoppingLists.Dtos;
using ShoppingLists.Entities;

namespace Web.Pages.ShoppingLists;

public class IndexModel : PageModel
{
    private readonly IShoppingListsService _shoppingListsService;
    private readonly IOperationsService _operationsService;

    public IndexModel(IShoppingListsService shoppingListsService, IOperationsService operationsService)
    {
        _shoppingListsService = shoppingListsService;
        _operationsService = operationsService;
    }
    
    public List<ShoppingList> ShoppingListsWithItems { get; private set; }
    
    public List<Card> AllCards { get; private set; }

    public async Task<IActionResult> OnGet()
    {
        var shoppingListsWithItems = await _shoppingListsService.GetAllUserShoppingListsWithItems(User);
        var userCards = await _operationsService.GetAllUserCards(User);

        ShoppingListsWithItems = shoppingListsWithItems?.OrderByDescending(list => list.CreationTime).ToList();
        AllCards = userCards?.OrderBy(card => card.Name).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostShoppingList(CreateShoppingListDto dto)
    {
        await _shoppingListsService.CreateShoppingList(User, dto);
        
        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostToOperations(CreatePaymentDto dto)
    {
        await _operationsService.CreatePayment(User, dto);

        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostUpdateShoppingList(UpdateShoppingListDto dto)
    {
        await _shoppingListsService.UpdateShoppingList(User, dto);
        
        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostDeleteShoppingList(Guid id)
    {
        await _shoppingListsService.DeleteShoppingList(User, id);
        
        return RedirectToPage();
    }
}