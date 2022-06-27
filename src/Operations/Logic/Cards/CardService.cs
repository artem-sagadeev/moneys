using Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Operations.Data;
using Operations.Dtos;
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

    public async Task<List<Card>> GetByUserId(string userId)
    {
        var cards = await _context.Cards.Where(card => card.UserId == userId).ToListAsync();
        
        if (cards is null || cards.Count == 0)
            throw new EntityNotFoundException();

        return cards;
    }

    public async Task<Guid> Create(CreateCardDto dto)
    {
        var card = new Card(dto);
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