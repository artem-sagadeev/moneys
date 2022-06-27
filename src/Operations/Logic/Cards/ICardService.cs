using Operations.Dtos;
using Operations.Entities;

namespace Operations.Logic.Cards;

public interface ICardService
{
    public Task<Card> GetById(Guid id);
    
    public Task<List<Card>> GetByUserId(string userId);

    public Task<Guid> Create(CreateCardDto dto);

    public Task Delete(Guid id);
}