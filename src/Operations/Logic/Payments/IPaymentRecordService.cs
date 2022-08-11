using Operations.Dtos;
using Operations.Dtos.PaymentRecords;
using Operations.Entities;

namespace Operations.Logic.Payments;

public interface IPaymentRecordService
{
    public Task<PaymentRecord> Get(Guid id);
    
    public Task<List<PaymentRecord>> GetByCardId(Guid cardId);
    
    public Task<List<PaymentRecord>> GetByCardIds(List<Guid> cardIds);

    public Task<Guid> Create(CreatePaymentRecordDto recordDto);

    public Task Update(UpdatePaymentRecordDto recordDto);

    public Task Delete(Guid id);
}