using Operations.Entities;

namespace Operations.Dtos.IncomeRecords;

public class CreateIncomeRecordDto
{
    public string Name { get; set; }
    
    public int Amount { get; set; }
    
    public Guid CardId { get; set; }
    
    public Guid? RegularIncomeId { get; set; }

    public CreateIncomeRecordDto(RegularIncome regularIncome)
    {
        Name = regularIncome.Name;
        Amount = regularIncome.Amount;
        CardId = regularIncome.CardId;
        RegularIncomeId = regularIncome.Id;
    }
    
    public CreateIncomeRecordDto() {}
}