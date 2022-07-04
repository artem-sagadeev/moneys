using Identity.Dtos;
using Identity.Entities;
using Identity.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly SignInManager<User> _signInManager;
    private readonly IAccountService _accountService;

    public LoginModel(SignInManager<User> signInManager, IAccountService accountService)
    {
        _signInManager = signInManager;
        _accountService = accountService;
    }
    
    public string ErrorMessage { get; private set; }

    public IActionResult OnGet()
    {
        if (_signInManager.IsSignedIn(User))
            return RedirectToPage("/Account/Index");

        return Page();
    }

    public async Task<IActionResult> OnPost(SignInDto dto)
    {
        if (_signInManager.IsSignedIn(User))
            return RedirectToPage("/Account/Index");
        
        var result = await _accountService.SignIn(dto);
        if (result.Succeeded)
        {
            return RedirectToPage("/Account/Index");
        }

        ErrorMessage = "Invalid login attempt";
        return Page();
    }
}