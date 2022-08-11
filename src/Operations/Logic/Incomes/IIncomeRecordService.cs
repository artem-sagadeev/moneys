using Operations.Dtos;
using Operations.Dtos.IncomeRecord;
using Operations.Entities;

namespace Operations.Logic.Incomes;

public interface IIncomeRecordService
{
    public Task<IncomeRecord> Get(Guid id);

    public Task<List<IncomeRecord>> GetByCardId(Guid cardId);
    
    public Task<List<IncomeRecord>> GetByCardIds(List<Guid> cardIds); 

    public Task<Guid> Create(CreateIncomeRecordDto dto);

    public Task Update(UpdateIncomeRecordDto dto);

    public Task Delete(Guid id);
}