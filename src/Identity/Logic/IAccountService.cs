using Identity.Dtos;
using Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Logic;

public interface IAccountService
{
    public Task<SignInResult> SignIn(SignInDto dto);

    public Task<User> SignUp(SignUpDto dto);

    public Task SignOut();

    public Task<bool> IsLoginUnique(string login);
}