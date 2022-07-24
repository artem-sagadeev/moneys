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

    public async Task<Payment> Get(Guid id)
    {
        var payment = await _context.Payments.GetById(id);

        return payment;
    }

    public async Task<List<Payment>> GetByCardId(Guid cardId)
    {
        await _context.Cards.CheckIfExists(cardId);

        var payments = await _context
            .Payments
            .Where(payment => payment.CardId == cardId)
            .ToListAsync();

        return payments;
    }

    public async Task<List<Payment>> GetByCardIds(List<Guid> cardIds)
    {
        var cards = await _context
            .Cards
            .Include(card => card.Payments)
            .Where(card => cardIds.Contains(card.Id))
            .ToListAsync();

        if (cards.Count != cardIds.Count)
            throw new EntityNotFoundException();

        var payments = cards
            .SelectMany(card => card.Payments)
            .ToList();
        
        return payments;
    }

    public async Task<Guid> Create(CreatePaymentDto dto)
    {
        var card = await _context.Cards.GetById(dto.CardId);

        if (dto.Amount > card.Balance)
            throw new NotEnoughMoneyException();

        var payment = new Payment(dto);
        _context.Payments.Add(payment);
        card.Balance -= payment.Amount;
        await _context.SaveChangesAsync();

        return payment.Id;
    }

    public async Task Update(UpdatePaymentDto dto)
    {
        var payment = await _context.Payments.GetById(dto.Id);

        if (dto.CardId != payment.CardId)
        {
            var currentCard = await _context.Cards.GetById(payment.CardId);
            var newCard = await _context.Cards.GetById(dto.CardId);

            if (newCard.Balance < payment.Amount)
                throw new NotEnoughMoneyException();
            
            payment.CardId = dto.CardId;
            currentCard.Balance += payment.Amount;
            newCard.Balance -= payment.Amount;
        }

        payment.Name = dto.Name;
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var payment = await _context
            .Payments
            .Include(payment => payment.Card)
            .SingleOrDefaultAsync(payment => payment.Id == id);

        if (payment is null)
            throw new EntityNotFoundException();

        payment.Card.Balance += payment.Amount;
        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync();
    }
}