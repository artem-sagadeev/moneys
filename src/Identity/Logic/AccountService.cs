using Common.Exceptions;
using Identity.Data;
using Identity.Dtos;
using Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Logic;

public class AccountService : IAccountService
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IdentityContext _context;

    public AccountService(SignInManager<User> signInManager, UserManager<User> userManager, IdentityContext context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
    }

    public async Task<SignInResult> SignIn(SignInDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Login))
            throw new FieldIsRequiredException();
        
        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new FieldIsRequiredException();
        
        var result = await _signInManager.PasswordSignInAsync(dto.Login, dto.Password, true, false);
        return result;
    }

    public async Task<User> SignUp(SignUpDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Login))
            throw new FieldIsRequiredException();
        
        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new FieldIsRequiredException();

        var existingUser = await _context.Users.SingleOrDefaultAsync(user => user.UserName == dto.Login);
        if (existingUser is not null)
            throw new AlreadyExistedException();
        
        var user = new User(dto);
        var result = await _userManager.CreateAsync(user, dto.Password);

        return result.Succeeded ? user : null;
    }

    public async Task SignOut()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<bool> IsLoginUnique(string login)
    {
        var isNotUnique = await _context
            .Users
            .AnyAsync(user => user.UserName == login);

        return !isNotUnique;
    }
}