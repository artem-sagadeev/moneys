using Common.Exceptions;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Operations.Data;
using Operations.Logic.Cards;

namespace OperationsTests;

public class CardTests
{
    private ICardService _cardService;
    private OperationsContext _context;
    
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<OperationsContext>()
            .UseInMemoryDatabase("moneys-db")
            .Options;
        
        using (var context = new OperationsContext(options))
        {
            context.Cards.AddRange(Seed.Cards);
            context.SaveChanges();
        }

        _context = new OperationsContext(options);
        _cardService = new CardService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Incomes.RemoveRange(_context.Incomes);
        _context.Payments.RemoveRange(_context.Payments);
        _context.Cards.RemoveRange(_context.Cards);
        _context.SaveChanges();
        _context.Dispose();
    }

    [Test]
    public async Task GetByIds_ReturnsCards()
    {
        var cards = Seed.Cards;

        var returnedCards = await _cardService.GetByIds(cards.Ids());
        
        Assert.That(returnedCards, Is.Not.Null);
        Assert.That(returnedCards, Has.Count.EqualTo(cards.Count));
        foreach (var card in cards)
        {
            Assert.That(returnedCards.Ids(), Contains.Item(card.Id));
        }
    }
    
    [Test]
    public void GetByIds_Throws_IfCardDoesNotExist()
    {
        var notExistedId = Seed.NotExistedCardId();
        var cardIds = Seed.Cards.Ids().Concat(new [] {notExistedId}).ToList();
        
        Assert.That(async () =>
        {
            await _cardService.GetByIds(cardIds);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }
    
    [Test]
    public async Task GetByUserId_ReturnsCards()
    {
        var userId = Seed.Cards[0].UserId;
        var cards = Seed.Cards.Where(card => card.UserId == userId).ToList();

        var returnedCards = await _cardService.GetByUserId(userId);
        
        Assert.That(returnedCards, Is.Not.Null);
        Assert.That(returnedCards, Has.Count.EqualTo(cards.Count));
        foreach (var card in cards)
        {
            Assert.That(returnedCards.Ids(), Contains.Item(card.Id));
        }
    }
}