using Operations.Entities;

namespace OperationsTests;

public static class Seed
{
    publi
    
    public static readonly List<Card> Cards = new()
    {
        new Card { Id = Guid.NewGuid(), Name = "Card 1", Balance = 1000, UserId = "test"}
    };

    public static readonly List<Income> Incomes = new()
    {
        new Income { Id = Guid.NewGuid(), Name = "Income 1", Amount = 100, DateTime = DateTime.UtcNow, CardId = Cards[0].Id }
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