using Common.Interfaces;
using Operations.Dtos;
using Operations.Interfaces;

namespace Operations.Entities;

public class Income : IOperation, IEntity
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int Amount { get; set; }
    
    public DateTime DateTime { get; set; }
    
    public Guid CardId { get; set; }
    
    public Card Card { get; set; }
    
    public Income(CreateIncomeDto dto)
    {
        Id = Guid.NewGuid();
        Name = dto.Name;
        Amount = dto.Amount;
        DateTime = DateTime.UtcNow;
        CardId = dto.CardId;
    }
    
    private Income() {}
}