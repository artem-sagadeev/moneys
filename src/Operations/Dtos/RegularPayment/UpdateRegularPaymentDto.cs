namespace Operations.Dtos.RegularPayment;

public class UpdateRegularPaymentDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int Amount { get; set; }
    
    public Guid CardId { get; set; }
}