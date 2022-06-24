using Identity.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Identity.Logic;

public interface IAccountService
{
    public Task<SignInResult> SignIn(SignInDto dto);

    public Task<IdentityResult> SignUp(SignUpDto dto);

    public Task SignOut();
}