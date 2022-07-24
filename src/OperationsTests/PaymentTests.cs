using Common.Exceptions;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Operations.Data;
using Operations.Dtos;
using Operations.Entities;
using Operations.Logic.Payments;

namespace OperationsTests;

public class PaymentTests
{
    private IPaymentService _paymentService;
    private OperationsContext _context;
    
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<OperationsContext>()
            .UseInMemoryDatabase("moneys-db")
            .Options;
        
        using (var context = new OperationsContext(options))
        {
            context.Payments.AddRange(Seed.Payments);
            context.Cards.AddRange(Seed.Cards);
            context.Cards.AddRange(Seed.CardsWithoutPayments);
            context.SaveChanges();
        }

        _context = new OperationsContext(options);
        _paymentService = new PaymentService(_context);
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
    public async Task Get_ReturnsPayment()
    {
        var payment = Seed.Payments[0];
        
        var returnedPayment = await _paymentService.Get(payment.Id);
        
        Assert.That(returnedPayment, Is.Not.Null);
        Assert.That(returnedPayment.Id, Is.EqualTo(payment.Id));
    }

    [Test]
    public void Get_Throws_IfPaymentDoesNotExist()
    {
        var notExistedId = Seed.NotExistedPaymentId();
        
        Assert.That(async () =>
        {
            await _paymentService.Get(notExistedId);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }

    [Test]
    public async Task GetByCardId_ReturnsPayments()
    {
        var card = Seed.Cards[0];
        var payments = Seed.Payments.Where(payment => payment.CardId == card.Id).ToList();

        var returnedPayments = await _paymentService.GetByCardId(card.Id);
        
        Assert.That(returnedPayments, Is.Not.Null);
        Assert.That(returnedPayments, Has.Count.EqualTo(payments.Count));
        foreach (var payment in payments)
        {
            Assert.That(returnedPayments.Ids(), Contains.Item(payment.Id));
        }
    }

    [Test]
    public void GetByCardId_Throws_IfCardDoesNotExist()
    {
        var notExistedId = Seed.NotExistedCardId();
        
        Assert.That(async () =>
        {
            await _paymentService.GetByCardId(notExistedId);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }

    [Test]
    public async Task GetByCardId_ReturnsEmptyList_IfThereIsNoPayments()
    {
        var card = Seed.CardsWithoutPayments[0];

        var returnedPayments = await _paymentService.GetByCardId(card.Id);
        
        Assert.That(returnedPayments, Is.TypeOf<List<Payment>>());
        Assert.That(returnedPayments, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetByCardIds_ReturnsPayments()
    {
        var cardIds = Seed.Cards.Ids();
        var payments = Seed.Payments.Where(payment => cardIds.Contains(payment.CardId)).ToList();
        
        var returnedPayments = await _paymentService.GetByCardIds(cardIds);
        
        Assert.That(returnedPayments, Is.Not.Null);
        Assert.That(returnedPayments, Has.Count.EqualTo(payments.Count));
        foreach (var payment in payments)
        {
            Assert.That(returnedPayments.Ids(), Contains.Item(payment.Id));
        }
    }
    
    [Test]
    public void GetByCardIds_Throws_IfCardDoesNotExist()
    {
        var notExistedId = Seed.NotExistedCardId();
        var cardIds = Seed.Cards.Ids().Concat(new [] {notExistedId}).ToList();
        
        Assert.That(async () =>
        {
            await _paymentService.GetByCardIds(cardIds);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }

    [Test]
    public async Task GetByCardIds_ReturnsEmptyList_IfThereIsNoPayments()
    {
        var cardIds = Seed.CardsWithoutPayments.Ids();

        var returnedPayments = await _paymentService.GetByCardIds(cardIds);
        
        Assert.That(returnedPayments, Is.TypeOf<List<Payment>>());
        Assert.That(returnedPayments, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task Create_CreatesPayment()
    {
        var dto = new CreatePaymentDto
        {
            Name = "New payment",
            Amount = 100,
            CardId = Seed.Cards[0].Id
        };

        var id = await _paymentService.Create(dto);

        var createdPayment = await _context.Payments.GetById(id);
        Assert.That(createdPayment, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(createdPayment.Name, Is.EqualTo(dto.Name));
            Assert.That(createdPayment.Amount, Is.EqualTo(dto.Amount));
            Assert.That(createdPayment.CardId, Is.EqualTo(dto.CardId));
            Assert.That(createdPayment.DateTime, Is.InRange(DateTime.UtcNow.AddSeconds(-1), 
                DateTime.UtcNow.AddSeconds(1)));
        });
    }

    [Test]
    public async Task Create_ReducesCardBalance()
    {
        var card = Seed.Cards[0];
        var dto = new CreatePaymentDto
        {
            Name = "New payment",
            Amount = 100,
            CardId = card.Id
        };

        await _paymentService.Create(dto);
        
        var updatedBalance = (await _context.Cards.GetById(card.Id)).Balance;
        Assert.That(updatedBalance, Is.EqualTo(card.Balance - dto.Amount));
    }

    [Test]
    public void Create_Throws_IfCardDoesNotExist()
    {
        var dto = new CreatePaymentDto
        {
            Name = "New payment",
            Amount = 100,
            CardId = Seed.NotExistedCardId()
        };

        Assert.That(async () =>
        {
            await _paymentService.Create(dto);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }
    
    [Test]
    public void Create_Throws_IfCardBalanceIsLessThanPaymentAmount()
    {
        var card = Seed.Cards[0];
        var dto = new CreatePaymentDto
        {
            Name = "New payment",
            Amount = 10000,
            CardId = card.Id
        };

        Assert.That(async () =>
        {
            await _paymentService.Create(dto);
        }, Throws.Exception.TypeOf<NotEnoughMoneyException>());
    }

    [Test]
    public async Task Update_ChangesName()
    {
        var payment = Seed.Payments[0];
        var dto = new UpdatePaymentDto
        {
            Id = payment.Id,
            Name = "New name",
            CardId = payment.CardId
        };

        await _paymentService.Update(dto);

        var updatedPayment = await _context.Payments.GetById(payment.Id);
        Assert.That(updatedPayment.Name, Is.EqualTo(dto.Name));
    }

    [Test]
    public async Task Update_ChangesCardsBalance_IfCardIdIsChanged()
    {
        var payment = Seed.Payments[0];
        var oldCard = Seed.Cards.Single(card => card.Id == payment.CardId);
        var newCard = Seed.CardsWithoutPayments[0];
        var dto = new UpdatePaymentDto
        {
            Id = payment.Id,
            Name = payment.Name,
            CardId = newCard.Id
        };
        
        await _paymentService.Update(dto);

        var updatedOldCard = await _context.Cards.GetById(oldCard.Id);
        var updatedNewCard = await _context.Cards.GetById(newCard.Id);
        Assert.Multiple(() =>
        {
            Assert.That(updatedOldCard.Balance, Is.EqualTo(oldCard.Balance + payment.Amount));
            Assert.That(updatedNewCard.Balance, Is.EqualTo(newCard.Balance - payment.Amount));
        });
    }

    [Test]
    public void Update_Throws_IfPaymentDoesNotExist()
    {
        var dto = new UpdatePaymentDto
        {
            Id = Seed.NotExistedPaymentId(),
            Name = "Some name",
            CardId = Seed.Cards[0].Id
        };
        
        Assert.That(async () =>
        {
            await _paymentService.Update(dto);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }

    [Test]
    public void Update_Throws_IfCardIdDoesNotExist()
    {
        var payment = Seed.Payments[0];
        var notExistedCardId = Seed.NotExistedCardId();
        var dto = new UpdatePaymentDto
        {
            Id = payment.Id,
            Name = payment.Name,
            CardId = notExistedCardId
        };
        
        Assert.That(async () =>
        {
            await _paymentService.Update(dto);
        }, Throws.Exception.TypeOf<EntityNotFoundException>()); 
    }

    [Test]
    public void Update_Throws_IfNewCardBalanceIsLessThanPaymentAmount()
    {
        var payment = Seed.PaymentWithLargeAmount;
        var dto = new UpdatePaymentDto
        {
            Id = payment.Id,
            Name = payment.Name,
            CardId = Seed.CardsWithoutPayments[0].Id
        };
        
        Assert.That(async () =>
        {
            await _paymentService.Update(dto);
        }, Throws.Exception.TypeOf<NotEnoughMoneyException>());
    }

    [Test]
    public async Task Delete_DeletesPayment()
    {
        var paymentId = Seed.Payments[0].Id;

        await _paymentService.Delete(paymentId);

        var deletedPayment = await _context.Payments.FindAsync(paymentId);
        Assert.That(deletedPayment, Is.Null);
    }
    
    [Test]
    public async Task Delete_ChangesCardBalance()
    {
        var payment = Seed.Payments[0];
        var card = Seed.Cards.Single(card => card.Id == payment.CardId);
        
        await _paymentService.Delete(payment.Id);

        var updatedCard = await _context.Cards.GetById(card.Id);
        Assert.That(updatedCard.Balance, Is.EqualTo(card.Balance + payment.Amount));
    }
    
    [Test]
    public void Delete_Throws_IfPaymentDoesNotExist()
    {
        var notExistedPaymentId = Seed.NotExistedPaymentId();

        Assert.That(async () =>
        {
            await _paymentService.Delete(notExistedPaymentId);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }
}