using Operations.Enums;
using Operations.Interfaces;

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
}