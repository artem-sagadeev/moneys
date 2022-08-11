using Common.Exceptions;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Operations.Data;
using Operations.Dtos.RegularPayment;
using Operations.Entities;

namespace Operations.Logic.Payments;

public class RegularPaymentService : IRegularPaymentService
{
    private readonly OperationsContext _context;

    public RegularPaymentService(OperationsContext context)
    {
        _context = context;
    }
    
    public async Task<RegularPayment> Get(Guid id)
    {
        var regularPayment = await _context.RegularPayments.GetById(id);

        return regularPayment;
    }

    public async Task<List<RegularPayment>> GetByCardId(Guid cardId)
    {
        await _context.Cards.CheckIfExists(cardId);

        var regularPayments = await _context
            .RegularPayments
            .Where(regularPayment => regularPayment.CardId == cardId)
            .ToListAsync();

        return regularPayments;
    }

    public async Task<List<RegularPayment>> GetByCardIds(List<Guid> cardIds)
    {
        var cards = await _context
            .Cards
            .Include(card => card.RegularPayments)
            .Where(card => cardIds.Contains(card.Id))
            .ToListAsync();

        if (cards.Count != cardIds.Count)
            throw new EntityNotFoundException();

        var regularPayments = cards
            .SelectMany(card => card.RegularPayments)
            .ToList();
        
        return regularPayments;
    }

    public async Task<Guid> Create(CreateRegularPaymentDto dto)
    {
        await _context.Cards.CheckIfExists(dto.CardId);
        
        var regularPayment = new RegularPayment(dto);
        _context.RegularPayments.Add(regularPayment);
        await _context.SaveChangesAsync();

        return regularPayment.Id;
    }

    public async Task Update(UpdateRegularPaymentDto dto)
    {
        var regularPayment = await _context.RegularPayments.GetById(dto.Id);

        if (regularPayment.CardId != dto.CardId)
        {
            regularPayment.CardId = dto.CardId;
        }

        regularPayment.Name = dto.Name;
        regularPayment.Amount = dto.Amount;

        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id, bool removePaymentRecords)
    {
        var regularPayment = await _context
            .RegularPayments
            .Include(regularPayment => regularPayment.PaymentRecords)
            .SingleOrDefaultAsync(regularPayment => regularPayment.Id == id);
        
        if (regularPayment is null)
            throw new EntityNotFoundException();

        if (removePaymentRecords)
        {
            _context.PaymentRecords.RemoveRange(regularPayment.PaymentRecords);
        }
        
        _context.RegularPayments.Remove(regularPayment);
        await _context.SaveChangesAsync();
    }
}