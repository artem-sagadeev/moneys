using ApplicationServices.ShoppingLists;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingLists.Dtos;
using ShoppingLists.Entities;

namespace Web.Pages.ShoppingLists;

public class IndexModel : PageModel
{
    private readonly IShoppingListsService _shoppingListsService;

    public IndexModel(IShoppingListsService shoppingListsService)
    {
        _shoppingListsService = shoppingListsService;
    }
    
    public List<ShoppingList> ShoppingListsWithItems { get; private set; }

    public async Task<IActionResult> OnGet()
    {
        var shoppingListsWithItems = await _shoppingListsService.GetAllUserShoppingListsWithItems(User);

        ShoppingListsWithItems = shoppingListsWithItems.OrderByDescending(list => list.CreationTime).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostShoppingList(CreateShoppingListDto dto)
    {
        await _shoppingListsService.CreateShoppingList(User, dto);
        
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