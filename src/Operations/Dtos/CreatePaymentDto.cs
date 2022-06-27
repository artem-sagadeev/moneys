namespace Operations.Dtos;

public class CreatePaymentDto
{
    public string Name { get; set; }
    
    public int Amount { get; set; }
    
    public Guid CardId { get; set; }
}