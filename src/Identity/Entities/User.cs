﻿using Identity.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Identity.Entities;

public class User : IdentityUser
{
    public sealed override string UserName { get; set; }
    
    public User(SignUpDto dto)
    {
        UserName = dto.Login;
    }
    
    private User() {}
}