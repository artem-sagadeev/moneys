using Operations.Enums;

namespace Operations.Dtos.RegularIncome;

public class CreateRegularIncomeDto
{
    public string Name { get; set; }
    
    public int Amount { get; set; }
    
    public Guid CardId { get; set; }
    
    public Frequency Frequency { get; set; }
}