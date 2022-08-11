using Common.Exceptions;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Operations.Data;
using Operations.Dtos;
using Operations.Dtos.PaymentRecords;
using Operations.Entities;

namespace Operations.Logic.Payments;

public class PaymentRecordService : IPaymentRecordService
{
    private readonly OperationsContext _context;

    public PaymentRecordService(OperationsContext context)
    {
        _context = context;
    }

    public async Task<PaymentRecord> Get(Guid id)
    {
        var paymentRecord = await _context.PaymentRecords.GetById(id);

        return paymentRecord;
    }

    public async Task<List<PaymentRecord>> GetByCardId(Guid cardId)
    {
        await _context.Cards.CheckIfExists(cardId);

        var payments = await _context
            .PaymentRecords
            .Where(paymentRecord => paymentRecord.CardId == cardId)
            .ToListAsync();

        return payments;
    }

    public async Task<List<PaymentRecord>> GetByCardIds(List<Guid> cardIds)
    {
        var cards = await _context
            .Cards
            .Include(card => card.PaymentRecords)
            .Where(card => cardIds.Contains(card.Id))
            .ToListAsync();

        if (cards.Count != cardIds.Count)
            throw new EntityNotFoundException();

        var payments = cards
            .SelectMany(card => card.PaymentRecords)
            .ToList();
        
        return payments;
    }

    public async Task<Guid> Create(CreatePaymentRecordDto recordDto)
    {
        var card = await _context.Cards.GetById(recordDto.CardId);

        if (recordDto.Amount > card.Balance)
            throw new NotEnoughMoneyException();

        var paymentRecord = new PaymentRecord(recordDto);
        _context.PaymentRecords.Add(paymentRecord);
        card.Balance -= paymentRecord.Amount;
        await _context.SaveChangesAsync();

        return paymentRecord.Id;
    }

    public async Task Update(UpdatePaymentRecordDto recordDto)
    {
        var paymentRecord = await _context.PaymentRecords.GetById(recordDto.Id);

        if (recordDto.CardId != paymentRecord.CardId)
        {
            var currentCard = await _context.Cards.GetById(paymentRecord.CardId);
            var newCard = await _context.Cards.GetById(recordDto.CardId);

            if (newCard.Balance < paymentRecord.Amount)
                throw new NotEnoughMoneyException();
            
            paymentRecord.CardId = recordDto.CardId;
            currentCard.Balance += paymentRecord.Amount;
            newCard.Balance -= paymentRecord.Amount;
        }

        paymentRecord.Name = recordDto.Name;
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var paymentRecord = await _context
            .PaymentRecords
            .Include(payment => payment.Card)
            .SingleOrDefaultAsync(payment => payment.Id == id);

        if (paymentRecord is null)
            throw new EntityNotFoundException();

        paymentRecord.Card.Balance += paymentRecord.Amount;
        _context.PaymentRecords.Remove(paymentRecord);
        await _context.SaveChangesAsync();
    }
}