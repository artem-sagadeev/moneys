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

    public async Task<List<Card>> GetByIds(List<Guid> ids)
    {
        var cards = await _context.Cards.Where(card => ids.Contains(card.Id)).ToListAsync();

        if (ids.Count != cards.Count)
            throw new EntityNotFoundException();

        return cards;
    }

    public async Task<List<Card>> GetByUserId(string userId)
    {
        var cards = await _context.Cards.Where(card => card.UserId == userId).ToListAsync();

        return cards;
    }

    public async Task<Guid> Create(CreateCardDto dto)
    {
        var card = new Card(dto);
        _context.Cards.Add(card);
        await _context.SaveChangesAsync();

        return card.Id;
    }

    public async Task Update(UpdateCardDto dto)
    {
        var card = await _context
            .Cards
            .Include(card => card.Payments)
            .SingleOrDefaultAsync(card => card.Id == dto.Id);

        if (card is null)
            throw new EntityNotFoundException();

        card.Name = dto.Name;
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var card = await _context
            .Cards
            .Include(card => card.Payments)
            .Include(card => card.Incomes)
            .SingleOrDefaultAsync(card => card.Id == id);

        if (card is null)
            throw new EntityNotFoundException();
        
        _context.Payments.RemoveRange(card.Payments);
        _context.Incomes.RemoveRange(card.Incomes);
        _context.Cards.Remove(card);
        await _context.SaveChangesAsync();
    }
}