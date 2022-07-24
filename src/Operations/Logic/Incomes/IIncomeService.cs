using Operations.Dtos;
using Operations.Entities;

namespace Operations.Logic.Incomes;

public interface IIncomeService
{
    public Task<IncomeRecord> Get(Guid id);

    public Task<List<IncomeRecord>> GetByCardId(Guid cardId);
    
    public Task<List<IncomeRecord>> GetByCardIds(List<Guid> cardIds); 

    public Task<Guid> Create(CreateIncomeRecordDto recordDto);

    public Task Update(UpdateIncomeRecordDto recordDto);

    public Task Delete(Guid id);
}