namespace Operations.Dtos;

public class TransferDto
{
    public string Name { get; set; }
    
    public decimal Amount { get; set; }
    
    public Guid FromCardId { get; set; }
    
    public Guid ToCardId { get; set; }
}