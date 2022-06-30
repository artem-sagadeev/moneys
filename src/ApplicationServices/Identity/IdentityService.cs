using System.Security.Claims;
using Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace ApplicationServices.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;

    public IdentityService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<User> GetUser(ClaimsPrincipal user)
    {
        var currentUser = await _userManager.GetUserAsync(user);

        return currentUser;
    }
}