using Common.Exceptions;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Operations.Data;
using Operations.Dtos.IncomeRecords;
using Operations.Dtos.RegularIncomes;
using Operations.Entities;

namespace Operations.Logic.Incomes;

public class RegularIncomeService : IRegularIncomeService
{
    private readonly OperationsContext _context;
    private readonly IIncomeRecordService _incomeRecordService;

    public RegularIncomeService(OperationsContext context, IIncomeRecordService incomeRecordService)
    {
        _context = context;
        _incomeRecordService = incomeRecordService;
    }

    public async Task<RegularIncome> Get(Guid id)
    {
        var regularIncome = await _context.RegularIncomes.GetById(id);

        return regularIncome;
    }

    public async Task<List<RegularIncome>> GetByCardId(Guid cardId)
    {
        await _context.Cards.CheckIfExists(cardId);

        var regularIncomes = await _context
            .RegularIncomes
            .Where(regularIncome => regularIncome.CardId == cardId)
            .ToListAsync();

        return regularIncomes;
    }

    public async Task<List<RegularIncome>> GetByCardIds(List<Guid> cardIds)
    {
        var cards = await _context
            .Cards
            .Include(card => card.RegularIncomes)
            .Where(card => cardIds.Contains(card.Id))
            .ToListAsync();

        if (cards.Count != cardIds.Count)
            throw new EntityNotFoundException();

        var regularIncomes = cards
            .SelectMany(card => card.RegularIncomes)
            .ToList();
        
        return regularIncomes;
    }

    public async Task<Guid> Create(CreateRegularIncomeDto dto)
    {
        await _context.Cards.CheckIfExists(dto.CardId);
        
        var regularIncome = new RegularIncome(dto);
        _context.RegularIncomes.Add(regularIncome);
        await _context.SaveChangesAsync();

        return regularIncome.Id;
    }

    public async Task Update(UpdateRegularIncomeDto dto)
    {
        var regularIncome = await _context.RegularIncomes.GetById(dto.Id);

        if (regularIncome.CardId != dto.CardId)
        {
            regularIncome.CardId = dto.CardId;
        }

        regularIncome.Name = dto.Name;
        regularIncome.Amount = dto.Amount;

        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id, bool removeIncomeRecords)
    {
        var regularIncome = await _context
            .RegularIncomes
            .Include(regularIncome => regularIncome.IncomeRecords)
            .SingleOrDefaultAsync(regularIncome => regularIncome.Id == id);
        
        if (regularIncome is null)
            throw new EntityNotFoundException();

        if (removeIncomeRecords)
        {
            _context.IncomeRecords.RemoveRange(regularIncome.IncomeRecords);
        }
        
        _context.RegularIncomes.Remove(regularIncome);
        await _context.SaveChangesAsync();
    }

    public async Task PerformRegularIncomes()
    {
        var regularIncomes = await _context
            .RegularIncomes
            .Where(regularIncome => regularIncome.IsActive && DateTime.Now > regularIncome.NextExecution)
            .ToListAsync();

        foreach (var regularIncome in regularIncomes)
        {
            await _incomeRecordService.Create(new CreateIncomeRecordDto(regularIncome));
            regularIncome.NextExecution = FrequencyHelper.CalculateNextExecution(DateTime.Now, regularIncome.Frequency);
        }
    }
}