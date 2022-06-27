using Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Operations.Data;
using Operations.Dtos;
using Operations.Entities;

namespace Operations.Logic.Incomes;

public class IncomeService : IIncomeService
{
    private readonly OperationsContext _context;

    public IncomeService(OperationsContext context)
    {
        _context = context;
    }

    public async Task<List<Income>> GetByCardId(Guid cardId)
    {
        var card = await _context.Cards.FindAsync(cardId);

        if (card is null)
            throw new EntityNotFoundException();

        var incomes = await _context
            .Incomes
            .Where(income => income.CardId == cardId)
            .ToListAsync();

        return incomes;
    }

    public async Task<List<Income>> GetByCardIds(List<Guid> cardIds)
    {
        var incomes = await _context
            .Incomes
            .Where(income => cardIds.Contains(income.CardId))
            .ToListAsync();

        return incomes;
    }

    public async Task<Guid> Create(CreateIncomeDto dto)
    {
        var card = await _context.Cards.FindAsync(dto.CardId);

        if (card is null)
            throw new EntityNotFoundException();

        var income = new Income(dto);
        _context.Incomes.Add(income);
        card.Balance += income.Amount;
        await _context.SaveChangesAsync();

        return income.Id;
    }

    public async Task Update(UpdateIncomeDto dto)
    {
        var income = await _context.Incomes.FindAsync(dto.Id);

        if (income is null)
            throw new EntityNotFoundException();

        income.Name = dto.Name;
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var income = await _context
            .Incomes
            .Include(income => income.Card)
            .SingleOrDefaultAsync(income => income.Id == id);

        if (income is null)
            throw new EntityNotFoundException();

        income.Card.Balance -= income.Amount;
        _context.Incomes.Remove(income);
        await _context.SaveChangesAsync();
    }
}