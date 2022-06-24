using Common.DTOs.Operations;
using Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Operations.Data;
using Operations.Entities;

namespace Operations.Logic.Payments;

public class PaymentService : IPaymentService
{
    private readonly OperationsContext _context;

    public PaymentService(OperationsContext context)
    {
        _context = context;
    }

    public async Task<List<Payment>> GetByCardId(Guid cardId)
    {
        var card = await _context.Cards.FindAsync(cardId);

        if (card is null)
            throw new EntityNotFoundException();

        var payments = await _context
            .Payments
            .Where(payment => payment.CardId == cardId)
            .ToListAsync();

        return payments;
    }

    public async Task<Guid> Create(CreatePaymentDto dto)
    {
        var card = await _context.Cards.FindAsync(dto.CardId);

        if (card is null)
            throw new EntityNotFoundException();

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
        var payment = await _context.Payments.FindAsync(dto.Id);

        if (payment is null)
            throw new EntityNotFoundException();

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