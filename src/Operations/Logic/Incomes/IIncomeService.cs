using Common.DTOs.Operations;
using Operations.Entities;

namespace Operations.Logic.Incomes;

public interface IIncomeService
{
    public Task<List<Income>> GetByCardId(Guid cardId);

    public Task<Guid> Create(CreateIncomeDto dto);

    public Task Update(UpdateIncomeDto dto);

    public Task Delete(Guid id);
}