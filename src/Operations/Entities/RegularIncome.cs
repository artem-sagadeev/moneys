using Operations.Dtos;
using Operations.Dtos.RegularIncomes;
using Operations.Enums;
using Operations.Interfaces;
using Operations.Logic;

namespace Operations.Entities;

public class RegularIncome : IRegularOperation
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int Amount { get; set; }
    
    public Guid CardId { get; set; }
    
    public Card Card { get; set; }
    
    public Frequency Frequency { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTime NextExecution { get; set; }
    
    public List<IncomeRecord> IncomeRecords { get; set; }
    
    public List<PaymentRecord> PaymentRecords { get; set; }

    public RegularIncome(CreateRegularIncomeDto dto)
    {
        Id = Guid.NewGuid();
        Name = dto.Name;
        Amount = dto.Amount;
        CardId = dto.CardId;
        Frequency = dto.Frequency;
        IsActive = true;
        NextExecution = FrequencyHelper.CalculateNextExecution(DateTime.Now, dto.Frequency);
    }
    
    private RegularIncome() {}
}