using Common.Exceptions;
using Common.Extensions;
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

    public async Task<Income> Get(Guid id)
    {
        var income = await _context.Incomes.GetById(id);

        return income;
    }

    public async Task<List<Income>> GetByCardId(Guid cardId)
    {
        await _context.Cards.CheckIfExists(cardId);

        var incomes = await _context
            .Incomes
            .Where(income => income.CardId == cardId)
            .ToListAsync();

        return incomes;
    }

    public async Task<List<Income>> GetByCardIds(List<Guid> cardIds)
    {
        var cards = await _context
            .Cards
            .Include(card => card.Incomes)
            .Where(card => cardIds.Contains(card.Id))
            .ToListAsync();

        if (cards.Count != cardIds.Count)
            throw new EntityNotFoundException();

        var incomes = cards
            .SelectMany(card => card.Incomes)
            .ToList();
        
        return incomes;
    }

    public async Task<Guid> Create(CreateIncomeDto dto)
    {
        var card = await _context.Cards.GetById(dto.CardId);

        var income = new Income(dto);
        _context.Incomes.Add(income);
        card.Balance += income.Amount;
        await _context.SaveChangesAsync();

        return income.Id;
    }

    public async Task Update(UpdateIncomeDto dto)
    {
        var income = await _context.Incomes.GetById(dto.Id);

        if (dto.CardId != income.CardId)
        {
            var currentCard = await _context.Cards.GetById(income.CardId);
            var newCard = await _context.Cards.GetById(dto.CardId);

            if (currentCard.Balance < income.Amount)
                throw new NotEnoughMoneyException();
            
            income.CardId = dto.CardId;
            currentCard.Balance -= income.Amount;
            newCard.Balance += income.Amount;
        }

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

        if (income.Card.Balance < income.Amount)
            throw new NotEnoughMoneyException();
        
        income.Card.Balance -= income.Amount;
        _context.Incomes.Remove(income);
        await _context.SaveChangesAsync();
    }
}