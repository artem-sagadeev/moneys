using Common.Exceptions;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Operations.Data;
using Operations.Logic.Incomes;

namespace OperationsTests;

public class IncomeTests
{
    private IIncomeService _incomeService;
    private OperationsContext _context;
    
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<OperationsContext>()
            .UseInMemoryDatabase("moneys-db")
            .Options;
        
        using (var context = new OperationsContext(options))
        {
            foreach (var income in Seed.Incomes)
            {
                context.Incomes.Add(income);
            }
            foreach (var card in Seed.Cards)
            {
                context.Cards.Add(card);
            }
            context.SaveChanges();
        }

        _context = new OperationsContext(options);
        _incomeService = new IncomeService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task Get_ReturnsIncomeIfExists()
    {
        var income = Seed.Incomes[0];
        
        var returnedIncome = await _incomeService.Get(income.Id);
        
        Assert.That(returnedIncome, Is.Not.Null);
        Assert.That(returnedIncome.Id, Is.EqualTo(income.Id));
    }

    [Test]
    public void Get_ThrowsIfDoesNotExist()
    {
        var notExistedId = Seed.NotExistedIncomeId();
        
        Assert.That(async () =>
        {
            await _incomeService.Get(notExistedId);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }

    [Test]
    public async Task GetByCardId_ReturnsIncomesIfExists()
    {
        var card = Seed.Cards[0];
        var incomes = Seed.Incomes.Where(income => income.CardId == card.Id).ToList();

        var returnedIncomes = await _incomeService.GetByCardId(card.Id);
        
        Assert.That(returnedIncomes, Is.Not.Null);
        Assert.That(returnedIncomes, Has.Count.EqualTo(incomes.Count));
        foreach (var income in incomes)
        {
            Assert.That(returnedIncomes.Ids(), Contains.Item(income.Id));
        }
    }

    [Test]
    public void GetByCardId_ThrowsIfCardDoesNotExist()
    {
        var notExistedId = Seed.NotExistedCardId();
        
        Assert.That(async () =>
        {
            await _incomeService.GetByCardId(notExistedId);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }

    [Test]
    public async Task GetByCardId_ReturnsEmptyListIfThereIsNoIncomes()
    {
        
    }
}