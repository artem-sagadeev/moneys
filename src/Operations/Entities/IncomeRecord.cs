using Operations.Dtos;
using Operations.Interfaces;

namespace Operations.Entities;

public class IncomeRecord : IOperationRecord
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int Amount { get; set; }
    
    public DateTime DateTime { get; set; }
    
    public Guid CardId { get; set; }
    
    public Card Card { get; set; }
    
    public Guid? RegularIncomeId { get; set; }
    
    public RegularIncome RegularIncome { get; set; }

    public bool IsRegularIncomeRecord => RegularIncomeId is not null;
    
    public IncomeRecord(CreateIncomeRecordDto recordDto)
    {
        Id = Guid.NewGuid();
        Name = recordDto.Name;
        Amount = recordDto.Amount;
        DateTime = DateTime.UtcNow;
        CardId = recordDto.CardId;
    }
    
    public IncomeRecord() {}
}