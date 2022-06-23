using Common.DTOs.Payments;
using Payments.Entities;

namespace Payments.Logic.Payments;

public interface IPaymentService
{
    public Task<List<Payment>> GetByCardId(Guid cardId);

    public Task<Guid> Create(CreatePaymentDto dto);

    public Task Update(UpdatePaymentDto dto);

    public Task Delete(Guid id);
}