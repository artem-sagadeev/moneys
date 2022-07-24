namespace Operations.Dtos;

public class UpdatePaymentRecordDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public Guid CardId { get; set; }
}