using Common.Exceptions;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Operations.Data;
using Operations.Dtos;
using Operations.Entities;
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
            context.IncomeRecords.AddRange(Seed.IncomeRecords);
            context.Cards.AddRange(Seed.Cards);
            context.Cards.AddRange(Seed.CardsWithoutIncomeRecords);
            context.SaveChanges();
        }

        _context = new OperationsContext(options);
        _incomeService = new IncomeService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.IncomeRecords.RemoveRange(_context.IncomeRecords);
        _context.PaymentRecords.RemoveRange(_context.PaymentRecords);
        _context.Cards.RemoveRange(_context.Cards);
        _context.SaveChanges();
        _context.Dispose();
    }

    [Test]
    public async Task Get_ReturnsIncome()
    {
        var income = Seed.IncomeRecords[0];
        
        var returnedIncome = await _incomeService.Get(income.Id);
        
        Assert.That(returnedIncome, Is.Not.Null);
        Assert.That(returnedIncome.Id, Is.EqualTo(income.Id));
    }

    [Test]
    public void Get_Throws_IfIncomeDoesNotExist()
    {
        var notExistedId = Seed.NotExistedIncomeRecordId();
        
        Assert.That(async () =>
        {
            await _incomeService.Get(notExistedId);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }

    [Test]
    public async Task GetByCardId_ReturnsIncomes()
    {
        var card = Seed.Cards[0];
        var incomes = Seed.IncomeRecords.Where(income => income.CardId == card.Id).ToList();

        var returnedIncomes = await _incomeService.GetByCardId(card.Id);
        
        Assert.That(returnedIncomes, Is.Not.Null);
        Assert.That(returnedIncomes, Has.Count.EqualTo(incomes.Count));
        foreach (var income in incomes)
        {
            Assert.That(returnedIncomes.Ids(), Contains.Item(income.Id));
        }
    }

    [Test]
    public void GetByCardId_Throws_IfCardDoesNotExist()
    {
        var notExistedId = Seed.NotExistedCardId();
        
        Assert.That(async () =>
        {
            await _incomeService.GetByCardId(notExistedId);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }

    [Test]
    public async Task GetByCardId_ReturnsEmptyList_IfThereIsNoIncomes()
    {
        var card = Seed.CardsWithoutIncomeRecords[0];

        var returnedIncomes = await _incomeService.GetByCardId(card.Id);
        
        Assert.That(returnedIncomes, Is.TypeOf<List<IncomeRecord>>());
        Assert.That(returnedIncomes, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetByCardIds_ReturnsIncomes()
    {
        var cardIds = Seed.Cards.Ids();
        var incomes = Seed.IncomeRecords.Where(income => cardIds.Contains(income.CardId)).ToList();
        
        var returnedIncomes = await _incomeService.GetByCardIds(cardIds);
        
        Assert.That(returnedIncomes, Is.Not.Null);
        Assert.That(returnedIncomes, Has.Count.EqualTo(incomes.Count));
        foreach (var income in incomes)
        {
            Assert.That(returnedIncomes.Ids(), Contains.Item(income.Id));
        }
    }
    
    [Test]
    public void GetByCardIds_Throws_IfCardDoesNotExist()
    {
        var notExistedId = Seed.NotExistedCardId();
        var cardIds = Seed.Cards.Ids().Concat(new [] {notExistedId}).ToList();
        
        Assert.That(async () =>
        {
            await _incomeService.GetByCardIds(cardIds);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }

    [Test]
    public async Task GetByCardIds_ReturnsEmptyList_IfThereIsNoIncomes()
    {
        var cardIds = Seed.CardsWithoutIncomeRecords.Ids();

        var returnedIncomes = await _incomeService.GetByCardIds(cardIds);
        
        Assert.That(returnedIncomes, Is.TypeOf<List<IncomeRecord>>());
        Assert.That(returnedIncomes, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task Create_CreatesIncome()
    {
        var dto = new CreateIncomeRecordDto
        {
            Name = "New income",
            Amount = 100,
            CardId = Seed.Cards[0].Id
        };

        var id = await _incomeService.Create(dto);

        var createdIncome = await _context.IncomeRecords.GetById(id);
        Assert.That(createdIncome, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(createdIncome.Name, Is.EqualTo(dto.Name));
            Assert.That(createdIncome.Amount, Is.EqualTo(dto.Amount));
            Assert.That(createdIncome.CardId, Is.EqualTo(dto.CardId));
            Assert.That(createdIncome.DateTime, Is.InRange(DateTime.UtcNow.AddSeconds(-1), 
                DateTime.UtcNow.AddSeconds(1)));
        });
    }

    [Test]
    public async Task Create_IncreasesCardBalance()
    {
        var card = Seed.Cards[0];
        var dto = new CreateIncomeRecordDto
        {
            Name = "New income",
            Amount = 100,
            CardId = card.Id
        };

        await _incomeService.Create(dto);
        
        var updatedBalance = (await _context.Cards.GetById(card.Id)).Balance;
        Assert.That(updatedBalance, Is.EqualTo(card.Balance + dto.Amount));
    }

    [Test]
    public void Create_Throws_IfCardDoesNotExist()
    {
        var dto = new CreateIncomeRecordDto
        {
            Name = "New income",
            Amount = 100,
            CardId = Seed.NotExistedCardId()
        };

        Assert.That(async () =>
        {
            await _incomeService.Create(dto);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }

    [Test]
    public async Task Update_ChangesName()
    {
        var income = Seed.IncomeRecords[0];
        var dto = new UpdateIncomeRecordDto
        {
            Id = income.Id,
            Name = "New name",
            CardId = income.CardId
        };

        await _incomeService.Update(dto);

        var updatedIncome = await _context.IncomeRecords.GetById(income.Id);
        Assert.That(updatedIncome.Name, Is.EqualTo(dto.Name));
    }

    [Test]
    public async Task Update_ChangesCardsBalance_IfCardIdIsChanged()
    {
        var income = Seed.IncomeRecords[0];
        var oldCard = Seed.Cards.Single(card => card.Id == income.CardId);
        var newCard = Seed.CardsWithoutIncomeRecords[0];
        var dto = new UpdateIncomeRecordDto
        {
            Id = income.Id,
            Name = income.Name,
            CardId = newCard.Id
        };
        
        await _incomeService.Update(dto);

        var updatedOldCard = await _context.Cards.GetById(oldCard.Id);
        var updatedNewCard = await _context.Cards.GetById(newCard.Id);
        Assert.Multiple(() =>
        {
            Assert.That(updatedOldCard.Balance, Is.EqualTo(oldCard.Balance - income.Amount));
            Assert.That(updatedNewCard.Balance, Is.EqualTo(newCard.Balance + income.Amount));
        });
    }

    [Test]
    public void Update_Throws_IfIncomeDoesNotExist()
    {
        var dto = new UpdateIncomeRecordDto
        {
            Id = Seed.NotExistedIncomeRecordId(),
            Name = "Some name",
            CardId = Seed.Cards[0].Id
        };
        
        Assert.That(async () =>
        {
            await _incomeService.Update(dto);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }

    [Test]
    public void Update_Throws_IfCardIdDoesNotExist()
    {
        var income = Seed.IncomeRecords[0];
        var notExistedCardId = Seed.NotExistedCardId();
        var dto = new UpdateIncomeRecordDto
        {
            Id = income.Id,
            Name = income.Name,
            CardId = notExistedCardId
        };
        
        Assert.That(async () =>
        {
            await _incomeService.Update(dto);
        }, Throws.Exception.TypeOf<EntityNotFoundException>()); 
    }

    [Test]
    public void Update_Throws_IfOldCardBalanceIsLessThanIncomeAmount()
    {
        var income = Seed.IncomeRecordWithLargeAmount;
        var dto = new UpdateIncomeRecordDto
        {
            Id = income.Id,
            Name = income.Name,
            CardId = Seed.CardsWithoutIncomeRecords[0].Id
        };
        
        Assert.That(async () =>
        {
            await _incomeService.Update(dto);
        }, Throws.Exception.TypeOf<NotEnoughMoneyException>());
    }

    [Test]
    public async Task Delete_DeletesIncome()
    {
        var incomeId = Seed.IncomeRecords[0].Id;

        await _incomeService.Delete(incomeId);

        var deletedIncome = await _context.IncomeRecords.FindAsync(incomeId);
        Assert.That(deletedIncome, Is.Null);
    }
    
    [Test]
    public async Task Delete_ChangesCardBalance()
    {
        var income = Seed.IncomeRecords[0];
        var card = Seed.Cards.Single(card => card.Id == income.CardId);
        
        await _incomeService.Delete(income.Id);

        var updatedCard = await _context.Cards.GetById(card.Id);
        Assert.That(updatedCard.Balance, Is.EqualTo(card.Balance - income.Amount));
    }
    
    [Test]
    public void Delete_Throws_IfIncomeDoesNotExist()
    {
        var notExistedIncomeId = Seed.NotExistedIncomeRecordId();

        Assert.That(async () =>
        {
            await _incomeService.Delete(notExistedIncomeId);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }
    
    [Test]
    public void Delte_Throws_IfCardBalanceIsLessThanIncomeAmount()
    {
        var incomeId = Seed.IncomeRecordWithLargeAmount.Id;
        
        Assert.That(async () =>
        {
            await _incomeService.Delete(incomeId);
        }, Throws.Exception.TypeOf<NotEnoughMoneyException>());
    }
}