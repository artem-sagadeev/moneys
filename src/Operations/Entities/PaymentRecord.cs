using Operations.Dtos;
using Operations.Dtos.PaymentRecord;
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

    public PaymentRecord(CreatePaymentRecordDto recordDto)
    {
        Id = Guid.NewGuid();
        Name = recordDto.Name;
        Amount = recordDto.Amount;
        DateTime = DateTime.UtcNow;
        CardId = recordDto.CardId;
    }
    
    public PaymentRecord() {}
}