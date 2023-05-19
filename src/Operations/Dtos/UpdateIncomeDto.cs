namespace Operations.Dtos;

public class UpdateIncomeDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public decimal Amount { get; set; }
    
    public Guid CardId { get; set; }
}