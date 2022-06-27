using System.ComponentModel.DataAnnotations;

namespace Identity.Dtos;

public class SignInDto
{
    public string Login { get; set; }
    
    public string Password { get; set; }
}