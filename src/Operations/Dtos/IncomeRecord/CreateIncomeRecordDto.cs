namespace Operations.Dtos.IncomeRecord;

public class CreateIncomeRecordDto
{
    public string Name { get; set; }
    
    public int Amount { get; set; }
    
    public Guid CardId { get; set; }
}