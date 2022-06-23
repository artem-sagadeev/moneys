using Common.DTOs.Payments;
using Payments.Entities;

namespace Payments.Logic.Incomes;

public interface IIncomeService
{
    public Task<List<Income>> GetByCardId(Guid cardId);

    public Task<Guid> Create(CreateIncomeDto dto);

    public Task Update(UpdateIncomeDto dto);

    public Task Delete(Guid id);
}