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

    public async Task<IncomeRecord> Get(Guid id)
    {
        var income = await _context.IncomeRecords.GetById(id);

        return income;
    }

    public async Task<List<IncomeRecord>> GetByCardId(Guid cardId)
    {
        await _context.Cards.CheckIfExists(cardId);

        var incomes = await _context
            .IncomeRecords
            .Where(income => income.CardId == cardId)
            .ToListAsync();

        return incomes;
    }

    public async Task<List<IncomeRecord>> GetByCardIds(List<Guid> cardIds)
    {
        var cards = await _context
            .Cards
            .Include(card => card.IncomeRecords)
            .Where(card => cardIds.Contains(card.Id))
            .ToListAsync();

        if (cards.Count != cardIds.Count)
            throw new EntityNotFoundException();

        var incomes = cards
            .SelectMany(card => card.IncomeRecords)
            .ToList();
        
        return incomes;
    }

    public async Task<Guid> Create(CreateIncomeRecordDto recordDto)
    {
        var card = await _context.Cards.GetById(recordDto.CardId);

        var income = new IncomeRecord(recordDto);
        _context.IncomeRecords.Add(income);
        card.Balance += income.Amount;
        await _context.SaveChangesAsync();

        return income.Id;
    }

    public async Task Update(UpdateIncomeRecordDto recordDto)
    {
        var income = await _context.IncomeRecords.GetById(recordDto.Id);

        if (recordDto.CardId != income.CardId)
        {
            var currentCard = await _context.Cards.GetById(income.CardId);
            var newCard = await _context.Cards.GetById(recordDto.CardId);

            if (currentCard.Balance < income.Amount)
                throw new NotEnoughMoneyException();
            
            income.CardId = recordDto.CardId;
            currentCard.Balance -= income.Amount;
            newCard.Balance += income.Amount;
        }

        income.Name = recordDto.Name;
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var income = await _context
            .IncomeRecords
            .Include(income => income.Card)
            .SingleOrDefaultAsync(income => income.Id == id);

        if (income is null)
            throw new EntityNotFoundException();

        if (income.Card.Balance < income.Amount)
            throw new NotEnoughMoneyException();
        
        income.Card.Balance -= income.Amount;
        _context.IncomeRecords.Remove(income);
        await _context.SaveChangesAsync();
    }
}