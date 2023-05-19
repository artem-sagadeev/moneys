namespace Operations.Interfaces;

public interface IOperation
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public decimal Amount { get; set; }
    
    public DateTime DateTime { get; set; }
    
    public Guid CardId { get; set; }
}