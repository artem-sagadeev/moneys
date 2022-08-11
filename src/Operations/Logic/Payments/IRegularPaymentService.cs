using Operations.Dtos.RegularPayment;
using Operations.Entities;

namespace Operations.Logic.Payments;

public interface IRegularPaymentService
{
    public Task<RegularPayment> Get(Guid id);

    public Task<List<RegularPayment>> GetByCardId(Guid cardId);
    
    public Task<List<RegularPayment>> GetByCardIds(List<Guid> cardIds); 

    public Task<Guid> Create(CreateRegularPaymentDto dto);

    public Task Update(UpdateRegularPaymentDto dto);

    public Task Delete(Guid id, bool removePaymentRecords);
}