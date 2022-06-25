using Identity.Entities;
using Identity.Logic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class SignOutModel : PageModel
{
    private readonly SignInManager<User> _signInManager;
    private readonly IAccountService _accountService;
    
    public SignOutModel(SignInManager<User> signInManager, IAccountService accountService)
    {
        _signInManager = signInManager;
        _accountService = accountService;
    }
    
    public async Task<IActionResult> OnGet()
    {
        if (_signInManager.IsSignedIn(User))
            await _accountService.SignOut();
            
        return RedirectToPage("Index");
    }
}