using Operations.Dtos;
using Operations.Dtos.IncomeRecords;
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
    
    public IncomeRecord(CreateIncomeRecordDto dto)
    {
        Id = Guid.NewGuid();
        Name = dto.Name;
        Amount = dto.Amount;
        DateTime = DateTime.UtcNow;
        CardId = dto.CardId;
        RegularIncomeId = dto.RegularIncomeId;
    }
    
    public IncomeRecord() {}
}