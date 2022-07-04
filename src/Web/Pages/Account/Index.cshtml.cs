using ApplicationServices.Identity;
using ApplicationServices.Operations;
using Identity.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Operations.Dtos;
using Operations.Entities;

namespace Web.Pages.Account;

public class IndexModel : PageModel
{
    private readonly IIdentityService _identityService;
    private readonly IOperationsService _operationsService;

    public IndexModel(IIdentityService identityService, IOperationsService operationsService)
    {
        _identityService = identityService;
        _operationsService = operationsService;
    }

    public User Profile { get; set; }
    
    public List<Card> Cards { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var user = await _identityService.GetUser(User);
        var cards = await _operationsService.GetAllUserCards(User);
        
        Profile = user;
        Cards = cards.OrderBy(card => card.Name).ToList();
        
        return Page();
    }

    public async Task<IActionResult> OnPostAddCard(CreateCardDto dto)
    {
        await _operationsService.CreateCard(User, dto);

        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostUpdateCard(UpdateCardDto dto)
    {
        await _operationsService.UpdateCard(User, dto);

        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostDeleteCard(Guid id)
    {
        await _operationsService.DeleteCard(User, id);

        return RedirectToPage();
    }
}