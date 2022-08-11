using Operations.Dtos;
using Operations.Dtos.PaymentRecords;
using Operations.Interfaces;

namespace Operations.Entities;

public class PaymentRecord : IOperationRecord
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int Amount { get; set; }
    
    public DateTime DateTime { get; set; }
    
    public Guid CardId { get; set; }
    
    public Card Card { get; set; }
    
    public Guid? RegularPaymentId { get; set; }
    
    public RegularPayment RegularPayment { get; set; }

    public bool IsRegularPaymentRecord => RegularPaymentId is not null;

    public PaymentRecord(CreatePaymentRecordDto dto)
    {
        Id = Guid.NewGuid();
        Name = dto.Name;
        Amount = dto.Amount;
        DateTime = DateTime.UtcNow;
        CardId = dto.CardId;
        RegularPaymentId = dto.RegularPaymentId;
    }
    
    public PaymentRecord() {}
}