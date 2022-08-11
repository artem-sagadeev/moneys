using Operations.Entities;

namespace Operations.Dtos.PaymentRecords;

public class CreatePaymentRecordDto
{
    public string Name { get; set; }
    
    public int Amount { get; set; }
    
    public Guid CardId { get; set; }
    
    public Guid? RegularPaymentId { get; set; }

    public CreatePaymentRecordDto(RegularPayment regularPayment)
    {
        Name = regularPayment.Name;
        Amount = regularPayment.Amount;
        CardId = regularPayment.CardId;
        RegularPaymentId = regularPayment.Id;

    }
    
    public CreatePaymentRecordDto() {}
}