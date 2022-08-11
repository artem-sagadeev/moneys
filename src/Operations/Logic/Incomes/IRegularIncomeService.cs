using Operations.Dtos;
using Operations.Dtos.RegularIncomes;
using Operations.Entities;

namespace Operations.Logic.Incomes;

public interface IRegularIncomeService
{
    public Task<RegularIncome> Get(Guid id);

    public Task<List<RegularIncome>> GetByCardId(Guid cardId);
    
    public Task<List<RegularIncome>> GetByCardIds(List<Guid> cardIds); 

    public Task<Guid> Create(CreateRegularIncomeDto dto);

    public Task Update(UpdateRegularIncomeDto dto);

    public Task Delete(Guid id, bool removeIncomeRecords);

    public Task PerformRegularIncomes();
}