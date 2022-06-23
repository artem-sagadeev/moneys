using Common.DTOs.Payments;

namespace Payments.Entities;

public class Income
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
        Amount = dto.Amount;
        DateTime = DateTime.Now;
        CardId = dto.CardId;
    }
    
    private Income() {}
}