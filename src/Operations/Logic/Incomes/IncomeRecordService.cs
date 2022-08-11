using Common.Exceptions;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Operations.Data;
using Operations.Dtos;
using Operations.Dtos.IncomeRecords;
using Operations.Entities;

namespace Operations.Logic.Incomes;

public class IncomeRecordService : IIncomeRecordService
{
    private readonly OperationsContext _context;

    public IncomeRecordService(OperationsContext context)
    {
        _context = context;
    }

    public async Task<IncomeRecord> Get(Guid id)
    {
        var incomeRecord = await _context.IncomeRecords.GetById(id);

        return incomeRecord;
    }

    public async Task<List<IncomeRecord>> GetByCardId(Guid cardId)
    {
        await _context.Cards.CheckIfExists(cardId);

        var incomeRecords = await _context
            .IncomeRecords
            .Where(incomeRecord => incomeRecord.CardId == cardId)
            .ToListAsync();

        return incomeRecords;
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

        var incomeRecords = cards
            .SelectMany(card => card.IncomeRecords)
            .ToList();
        
        return incomeRecords;
    }

    public async Task<Guid> Create(CreateIncomeRecordDto dto)
    {
        var card = await _context.Cards.GetById(dto.CardId);

        var incomeRecord = new IncomeRecord(dto);
        _context.IncomeRecords.Add(incomeRecord);
        card.Balance += incomeRecord.Amount;
        await _context.SaveChangesAsync();

        return incomeRecord.Id;
    }

    public async Task Update(UpdateIncomeRecordDto dto)
    {
        var incomeRecord = await _context.IncomeRecords.GetById(dto.Id);

        if (dto.CardId != incomeRecord.CardId)
        {
            var currentCard = await _context.Cards.GetById(incomeRecord.CardId);
            var newCard = await _context.Cards.GetById(dto.CardId);

            if (currentCard.Balance < incomeRecord.Amount)
                throw new NotEnoughMoneyException();
            
            incomeRecord.CardId = dto.CardId;
            currentCard.Balance -= incomeRecord.Amount;
            newCard.Balance += incomeRecord.Amount;
        }

        incomeRecord.Name = dto.Name;
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var incomeRecord = await _context
            .IncomeRecords
            .Include(incomeRecord => incomeRecord.Card)
            .SingleOrDefaultAsync(incomeRecord => incomeRecord.Id == id);

        if (incomeRecord is null)
            throw new EntityNotFoundException();

        if (incomeRecord.Card.Balance < incomeRecord.Amount)
            throw new NotEnoughMoneyException();
        
        incomeRecord.Card.Balance -= incomeRecord.Amount;
        _context.IncomeRecords.Remove(incomeRecord);
        await _context.SaveChangesAsync();
    }
}