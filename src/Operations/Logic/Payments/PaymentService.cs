using Common.Exceptions;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Operations.Data;
using Operations.Dtos;
using Operations.Entities;

namespace Operations.Logic.Payments;

public class PaymentService : IPaymentService
{
    private readonly OperationsContext _context;

    public PaymentService(OperationsContext context)
    {
        _context = context;
    }

    public async Task<PaymentRecord> Get(Guid id)
    {
        var payment = await _context.PaymentRecords.GetById(id);

        return payment;
    }

    public async Task<List<PaymentRecord>> GetByCardId(Guid cardId)
    {
        await _context.Cards.CheckIfExists(cardId);

        var payments = await _context
            .PaymentRecords
            .Where(payment => payment.CardId == cardId)
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

        var payment = new PaymentRecord(recordDto);
        _context.PaymentRecords.Add(payment);
        card.Balance -= payment.Amount;
        await _context.SaveChangesAsync();

        return payment.Id;
    }

    public async Task Update(UpdatePaymentRecordDto recordDto)
    {
        var payment = await _context.PaymentRecords.GetById(recordDto.Id);

        if (recordDto.CardId != payment.CardId)
        {
            var currentCard = await _context.Cards.GetById(payment.CardId);
            var newCard = await _context.Cards.GetById(recordDto.CardId);

            if (newCard.Balance < payment.Amount)
                throw new NotEnoughMoneyException();
            
            payment.CardId = recordDto.CardId;
            currentCard.Balance += payment.Amount;
            newCard.Balance -= payment.Amount;
        }

        payment.Name = recordDto.Name;
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var payment = await _context
            .PaymentRecords
            .Include(payment => payment.Card)
            .SingleOrDefaultAsync(payment => payment.Id == id);

        if (payment is null)
            throw new EntityNotFoundException();

        payment.Card.Balance += payment.Amount;
        _context.PaymentRecords.Remove(payment);
        await _context.SaveChangesAsync();
    }
}