using System.Security.Claims;
using Identity.Entities;

namespace ApplicationServices.Identity;

public interface IIdentityService
{
    public Task<User> GetUser(ClaimsPrincipal user);
}