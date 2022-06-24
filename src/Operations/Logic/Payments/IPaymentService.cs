using Common.DTOs.Operations;
using Operations.Entities;

namespace Operations.Logic.Payments;

public interface IPaymentService
{
    public Task<List<Payment>> GetByCardId(Guid cardId);

    public Task<Guid> Create(CreatePaymentDto dto);

    public Task Update(UpdatePaymentDto dto);

    public Task Delete(Guid id);
}