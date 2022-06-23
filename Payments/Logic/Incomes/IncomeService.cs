using Common.DTOs.Payments;
using Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Payments.Data;
using Payments.Entities;

namespace Payments.Logic.Incomes;

public class IncomeService : IIncomeService
{
    private readonly PaymentsContext _context;

    public IncomeService(PaymentsContext context)
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

    public async Task<Guid> Create(CreateIncomeDto dto)
    {
        var card = await _context.Cards.FindAsync(dto.CardId);

        if (card is null)
            throw new EntityNotFoundException();

        if (dto.Amount > card.Balance)
            throw new NotEnoughMoneyException();

        var income = new Income(dto);
        _context.Incomes.Add(income);
        card.Balance += income.Amount;

        return income.Id;
    }

    public async Task Update(UpdateIncomeDto dto)
    {
        var income = await _context.Payments.FindAsync(dto.Id);

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