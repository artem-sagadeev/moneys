using System.ComponentModel.DataAnnotations;

namespace Identity.Dtos;

public class SignInDto
{
    public string Login { get; set; }
    
    public string Password { get; set; }
    
    public string RememberMe { get; set; }

    public bool IsPersistent => RememberMe == "on";
}