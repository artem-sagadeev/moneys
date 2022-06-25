using Identity.Entities;
using Identity.Logic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class ProfileModel : PageModel
{
    private readonly SignInManager<User> _signInManager;

    public ProfileModel(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    public User Profile { get; set; }
    
    public async Task<IActionResult> OnGet()
    {
        if (!_signInManager.IsSignedIn(User))
            return Redirect("SignIn");
        
        Profile = await _signInManager.UserManager.GetUserAsync(User);
        return Page();
    }
}