using Identity.Dtos;
using Identity.Entities;
using Identity.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Account;

[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly SignInManager<User> _signInManager;
    private readonly IAccountService _accountService;

    public RegisterModel(SignInManager<User> signInManager, IAccountService accountService)
    {
        _signInManager = signInManager;
        _accountService = accountService;
    }

    public IActionResult OnGet()
    {
        if (_signInManager.IsSignedIn(User))
            return RedirectToPage("/Account/Index");

        return Page();
    }

    public async Task<IActionResult> OnPost(SignUpDto dto)
    {
        if (_signInManager.IsSignedIn(User))
            return RedirectToPage("/Account/Index");
        
        var user = await _accountService.SignUp(dto);
        if (user is not null)
        {
            await _signInManager.SignInAsync(user, true);
            return RedirectToPage("/Account/Index");
        }
        
        return Page();
    }

    public async Task<IActionResult> OnGetUnique(string login)
    {
        var isUnique = await _accountService.IsLoginUnique(login);

        return Content(isUnique.ToString());
    }
}