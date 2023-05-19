namespace Operations.Dtos;

public class CreatePaymentDto
{
    public string Name { get; set; }
    
    public decimal Amount { get; set; }
    
    public Guid CardId { get; set; }
    
    public Guid ListId { get; set; }
}