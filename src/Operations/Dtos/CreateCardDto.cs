namespace Operations.Dtos;

public class CreateCardDto
{
    public string Name { get; set; }

    public decimal StartBalance { get; set; }
    
    public string UserId { get; set; }
}