using Operations.Dtos;
using Operations.Entities;

namespace Operations.Logic.Payments;

public interface IPaymentService
{
    public Task<Payment> Get(Guid id);
    
    public Task<List<Payment>> GetByCardId(Guid cardId);
    
    public Task<List<Payment>> GetByCardIds(List<Guid> cardIds);

    public Task<Guid> Create(CreatePaymentDto dto);

    public Task Update(UpdatePaymentDto dto);

    public Task Delete(Guid id);
}