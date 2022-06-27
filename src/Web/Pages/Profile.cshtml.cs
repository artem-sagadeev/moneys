using Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Operations.Dtos;
using Operations.Entities;
using Operations.Logic.Cards;

namespace Web.Pages;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly SignInManager<User> _signInManager;
    private readonly ICardService _cardService;

    public ProfileModel(SignInManager<User> signInManager, ICardService cardService)
    {
        _signInManager = signInManager;
        _cardService = cardService;
    }

    public User Profile { get; set; }
    
    public List<Card> Cards { get; set; }

    public async Task<IActionResult> OnGet()
    {
        if (!_signInManager.IsSignedIn(User))
            return Redirect("SignIn");

        var user = await _signInManager.UserManager.GetUserAsync(User);
        Profile = user;
        Cards = await _cardService.GetByUserId(user.Id);
        return Page();
    }

    public async Task<IActionResult> OnPostAddCard(CreateCardDto dto)
    {
        if (!_signInManager.IsSignedIn(User))
            return Redirect("SignIn");

        var user = await _signInManager.UserManager.GetUserAsync(User);
        dto.UserId = user.Id;
        await _cardService.Create(dto);

        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostUpdateCard(UpdateCardDto dto)
    {
        if (!_signInManager.IsSignedIn(User))
            return Redirect("SignIn");

        await _cardService.Update(dto);

        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostDeleteCard(Guid id)
    {
        await _cardService.Delete(id);

        return RedirectToPage();
    }
}