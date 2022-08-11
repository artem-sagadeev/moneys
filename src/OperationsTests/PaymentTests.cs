using Common.Exceptions;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Operations.Data;
using Operations.Dtos;
using Operations.Dtos.PaymentRecord;
using Operations.Entities;
using Operations.Logic.Payments;

namespace OperationsTests;

public class PaymentTests
{
    private IPaymentRecordService _paymentRecordService;
    private OperationsContext _context;
    
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<OperationsContext>()
            .UseInMemoryDatabase("moneys-db")
            .Options;
        
        using (var context = new OperationsContext(options))
        {
            context.PaymentRecords.AddRange(Seed.PaymentRecords);
            context.Cards.AddRange(Seed.Cards);
            context.Cards.AddRange(Seed.CardsWithoutPaymentRecords);
            context.SaveChanges();
        }

        _context = new OperationsContext(options);
        _paymentRecordService = new PaymentRecordService(_context);
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
    public async Task Get_ReturnsPayment()
    {
        var payment = Seed.PaymentRecords[0];
        
        var returnedPayment = await _paymentRecordService.Get(payment.Id);
        
        Assert.That(returnedPayment, Is.Not.Null);
        Assert.That(returnedPayment.Id, Is.EqualTo(payment.Id));
    }

    [Test]
    public void Get_Throws_IfPaymentDoesNotExist()
    {
        var notExistedId = Seed.NotExistedPaymentRecordId();
        
        Assert.That(async () =>
        {
            await _paymentRecordService.Get(notExistedId);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }

    [Test]
    public async Task GetByCardId_ReturnsPayments()
    {
        var card = Seed.Cards[0];
        var payments = Seed.PaymentRecords.Where(payment => payment.CardId == card.Id).ToList();

        var returnedPayments = await _paymentRecordService.GetByCardId(card.Id);
        
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
            await _paymentRecordService.GetByCardId(notExistedId);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }

    [Test]
    public async Task GetByCardId_ReturnsEmptyList_IfThereIsNoPayments()
    {
        var card = Seed.CardsWithoutPaymentRecords[0];

        var returnedPayments = await _paymentRecordService.GetByCardId(card.Id);
        
        Assert.That(returnedPayments, Is.TypeOf<List<PaymentRecord>>());
        Assert.That(returnedPayments, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetByCardIds_ReturnsPayments()
    {
        var cardIds = Seed.Cards.Ids();
        var payments = Seed.PaymentRecords.Where(payment => cardIds.Contains(payment.CardId)).ToList();
        
        var returnedPayments = await _paymentRecordService.GetByCardIds(cardIds);
        
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
            await _paymentRecordService.GetByCardIds(cardIds);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }

    [Test]
    public async Task GetByCardIds_ReturnsEmptyList_IfThereIsNoPayments()
    {
        var cardIds = Seed.CardsWithoutPaymentRecords.Ids();

        var returnedPayments = await _paymentRecordService.GetByCardIds(cardIds);
        
        Assert.That(returnedPayments, Is.TypeOf<List<PaymentRecord>>());
        Assert.That(returnedPayments, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task Create_CreatesPayment()
    {
        var dto = new CreatePaymentRecordDto
        {
            Name = "New payment",
            Amount = 100,
            CardId = Seed.Cards[0].Id
        };

        var id = await _paymentRecordService.Create(dto);

        var createdPayment = await _context.PaymentRecords.GetById(id);
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
        var dto = new CreatePaymentRecordDto
        {
            Name = "New payment",
            Amount = 100,
            CardId = card.Id
        };

        await _paymentRecordService.Create(dto);
        
        var updatedBalance = (await _context.Cards.GetById(card.Id)).Balance;
        Assert.That(updatedBalance, Is.EqualTo(card.Balance - dto.Amount));
    }

    [Test]
    public void Create_Throws_IfCardDoesNotExist()
    {
        var dto = new CreatePaymentRecordDto
        {
            Name = "New payment",
            Amount = 100,
            CardId = Seed.NotExistedCardId()
        };

        Assert.That(async () =>
        {
            await _paymentRecordService.Create(dto);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }
    
    [Test]
    public void Create_Throws_IfCardBalanceIsLessThanPaymentAmount()
    {
        var card = Seed.Cards[0];
        var dto = new CreatePaymentRecordDto
        {
            Name = "New payment",
            Amount = 10000,
            CardId = card.Id
        };

        Assert.That(async () =>
        {
            await _paymentRecordService.Create(dto);
        }, Throws.Exception.TypeOf<NotEnoughMoneyException>());
    }

    [Test]
    public async Task Update_ChangesName()
    {
        var payment = Seed.PaymentRecords[0];
        var dto = new UpdatePaymentRecordDto
        {
            Id = payment.Id,
            Name = "New name",
            CardId = payment.CardId
        };

        await _paymentRecordService.Update(dto);

        var updatedPayment = await _context.PaymentRecords.GetById(payment.Id);
        Assert.That(updatedPayment.Name, Is.EqualTo(dto.Name));
    }

    [Test]
    public async Task Update_ChangesCardsBalance_IfCardIdIsChanged()
    {
        var payment = Seed.PaymentRecords[0];
        var oldCard = Seed.Cards.Single(card => card.Id == payment.CardId);
        var newCard = Seed.CardsWithoutPaymentRecords[0];
        var dto = new UpdatePaymentRecordDto
        {
            Id = payment.Id,
            Name = payment.Name,
            CardId = newCard.Id
        };
        
        await _paymentRecordService.Update(dto);

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
        var dto = new UpdatePaymentRecordDto
        {
            Id = Seed.NotExistedPaymentRecordId(),
            Name = "Some name",
            CardId = Seed.Cards[0].Id
        };
        
        Assert.That(async () =>
        {
            await _paymentRecordService.Update(dto);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }

    [Test]
    public void Update_Throws_IfCardIdDoesNotExist()
    {
        var payment = Seed.PaymentRecords[0];
        var notExistedCardId = Seed.NotExistedCardId();
        var dto = new UpdatePaymentRecordDto
        {
            Id = payment.Id,
            Name = payment.Name,
            CardId = notExistedCardId
        };
        
        Assert.That(async () =>
        {
            await _paymentRecordService.Update(dto);
        }, Throws.Exception.TypeOf<EntityNotFoundException>()); 
    }

    [Test]
    public void Update_Throws_IfNewCardBalanceIsLessThanPaymentAmount()
    {
        var payment = Seed.PaymentRecordWithLargeAmount;
        var dto = new UpdatePaymentRecordDto
        {
            Id = payment.Id,
            Name = payment.Name,
            CardId = Seed.CardsWithoutPaymentRecords[0].Id
        };
        
        Assert.That(async () =>
        {
            await _paymentRecordService.Update(dto);
        }, Throws.Exception.TypeOf<NotEnoughMoneyException>());
    }

    [Test]
    public async Task Delete_DeletesPayment()
    {
        var paymentId = Seed.PaymentRecords[0].Id;

        await _paymentRecordService.Delete(paymentId);

        var deletedPayment = await _context.PaymentRecords.FindAsync(paymentId);
        Assert.That(deletedPayment, Is.Null);
    }
    
    [Test]
    public async Task Delete_ChangesCardBalance()
    {
        var payment = Seed.PaymentRecords[0];
        var card = Seed.Cards.Single(card => card.Id == payment.CardId);
        
        await _paymentRecordService.Delete(payment.Id);

        var updatedCard = await _context.Cards.GetById(card.Id);
        Assert.That(updatedCard.Balance, Is.EqualTo(card.Balance + payment.Amount));
    }
    
    [Test]
    public void Delete_Throws_IfPaymentDoesNotExist()
    {
        var notExistedPaymentId = Seed.NotExistedPaymentRecordId();

        Assert.That(async () =>
        {
            await _paymentRecordService.Delete(notExistedPaymentId);
        }, Throws.Exception.TypeOf<EntityNotFoundException>());
    }
}