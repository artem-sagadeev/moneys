using Operations.Dtos;
using Operations.Entities;

namespace Operations.Logic.Incomes;

public interface IIncomeService
{
    public Task<Income> Get(Guid id);

    public Task<List<Income>> GetByCardId(Guid cardId);
    
    public Task<List<Income>> GetByCardIds(List<Guid> cardIds); 

    public Task<Guid> Create(CreateIncomeDto dto);

    public Task Update(UpdateIncomeDto dto);

    public Task Delete(Guid id);
}