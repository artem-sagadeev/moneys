using Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Operations.Dtos;
using Operations.Logic.Cards;

namespace Web.Pages;

public class CreateCardModel : PageModel
{
    private readonly ICardService _cardService;
    private readonly SignInManager<User> _signInManager;

    public CreateCardModel(ICardService cardService, SignInManager<User> signInManager)
    {
        _cardService = cardService;
        _signInManager = signInManager;
    }

    public void OnGet()
    {
        
    }

    public async Task<IActionResult> OnPost(CreateCardDto dto)
    {
        if (!_signInManager.IsSignedIn(User))
            return Forbid();

        var user = await _signInManager.UserManager.GetUserAsync(User);
        dto.UserId = user.Id;
        await _cardService.Create(dto);

        return RedirectToPage("Operations");
    }
}