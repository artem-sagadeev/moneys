namespace Identity.Dtos;

public class SignUpDto
{
    public string Login { get; set; }
    public string Password { get; set; }
    
    public string RememberMe { get; set; }

    public bool IsPersistent => RememberMe == "on";
}