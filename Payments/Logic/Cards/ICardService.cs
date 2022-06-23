using Payments.Entities;

namespace Payments.Logic.Cards;

public interface ICardService
{
    public Task<Card> GetById(Guid id);

    public Task<Guid> Create();

    public Task Delete(Guid id);
}