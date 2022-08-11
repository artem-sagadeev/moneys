namespace Operations.Dtos.Cards;

public class CreateCardDto
{
    public string Name { get; set; }
    
    public int StartBalance { get; set; }
    
    public string UserId { get; set; }
}