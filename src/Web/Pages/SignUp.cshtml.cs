﻿using Identity.Dtos;
using Identity.Entities;
using Identity.Logic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class SignUpModel : PageModel
{
    private readonly SignInManager<User> _signInManager;
    private readonly IAccountService _accountService;

    public SignUpModel(SignInManager<User> signInManager, IAccountService accountService)
    {
        _signInManager = signInManager;
        _accountService = accountService;
    }
    
    public List<string> Errors { get; private set; }

    public IActionResult OnGet()
    {
        if (_signInManager.IsSignedIn(User))
            return RedirectToPage("Profile");

        return Page();
    }

    public async Task<IActionResult> OnPost(SignUpDto dto)
    {
        if (_signInManager.IsSignedIn(User))
            return RedirectToPage("Profile");
        
        var result = await _accountService.SignUp(dto);
        if (result.Succeeded)
            return RedirectToPage("Profile");

        Errors = new List<string>(result.Errors.Select(error => error.Description));
        return Page();
    }
}