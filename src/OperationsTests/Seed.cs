using Operations.Entities;

namespace OperationsTests;

public static class Seed
{
    public static readonly List<Card> CardsWithoutIncomes = new()
    {
        new Card {Id = Guid.NewGuid(), Name = "Card without incomes 1", Balance = 1000, UserId = "test"},
        new Card {Id = Guid.NewGuid(), Name = "Card without incomes 2", Balance = 2000, UserId = "test"},
        new Card {Id = Guid.NewGuid(), Name = "Card without incomes 3", Balance = 3000, UserId = "test"}
    };
    
    public static readonly List<Card> Cards = new()
    {
        new Card {Id = Guid.NewGuid(), Name = "Card 1", Balance = 1000, UserId = "test"},
        new Card {Id = Guid.NewGuid(), Name = "Card 2", Balance = 2000, UserId = "test"},
        new Card {Id = Guid.NewGuid(), Name = "Card 3", Balance = 3000, UserId = "test"}
    };

    public static readonly Income IncomeWithLargeAmount =
        new () {Id = Guid.NewGuid(), Name = "Income with large amount", Amount = 10000, DateTime = DateTime.UtcNow, CardId = Cards[0].Id};
    
    public static readonly List<Income> Incomes = new()
    {
        new Income {Id = Guid.NewGuid(), Name = "Income 1", Amount = 100, DateTime = DateTime.UtcNow, CardId = Cards[0].Id},
        new Income {Id = Guid.NewGuid(), Name = "Income 2", Amount = 200, DateTime = DateTime.UtcNow, CardId = Cards[1].Id},
        new Income {Id = Guid.NewGuid(), Name = "Income 3", Amount = 300, DateTime = DateTime.UtcNow, CardId = Cards[2].Id},
        IncomeWithLargeAmount
    };

    public static Guid NotExistedCardId()
    {
        var notExistedId = Guid.NewGuid();
        while (Cards.Select(card => card.Id).Contains(notExistedId))
        {
            notExistedId = Guid.NewGuid();
        }

        return notExistedId;
    }
    
    public static Guid NotExistedIncomeId()
    {
        var notExistedId = Guid.NewGuid();
        while (Incomes.Select(income => income.Id).Contains(notExistedId))
        {
            notExistedId = Guid.NewGuid();
        }

        return notExistedId;
    }
}