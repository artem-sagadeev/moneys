using Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Operations.Data;
using Operations.Entities;

namespace Operations.Logic.Cards;

public class CardService : ICardService
{
    private readonly OperationsContext _context;

    public CardService(OperationsContext context)
    {
        _context = context;
    }

    public async Task<Card> GetById(Guid id)
    {
        var card = await _context.Cards.FindAsync(id);

        if (card is null)
            throw new EntityNotFoundException();

        return card;
    }

    public async Task<Guid> Create()
    {
        var card = new Card(0);
        _context.Cards.Add(card);
        await _context.SaveChangesAsync();

        return card.Id;
    }

    public async Task Delete(Guid id)
    {
        var card = await _context
            .Cards
            .Include(card => card.Payments)
            .SingleOrDefaultAsync(card => card.Id == id);

        if (card is null)
            throw new EntityNotFoundException();

        _context.Payments.RemoveRange(card.Payments);
        _context.Incomes.RemoveRange(card.Incomes);
        _context.Cards.Remove(card);
        await _context.SaveChangesAsync();
    }
}