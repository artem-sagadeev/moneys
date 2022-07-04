using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

[AllowAnonymous]
public class Index : PageModel
{
    public void OnGet()
    {
        
    }
}