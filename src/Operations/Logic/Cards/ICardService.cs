using Operations.Dtos;
using Operations.Entities;

namespace Operations.Logic.Cards;

public interface ICardService
{
    public Task<List<Card>> GetByIds(List<Guid> ids);
    
    public Task<List<Card>> GetByUserId(string userId);

    public Task<Guid> Create(CreateCardDto dto);

    public Task Update(UpdateCardDto dto);

    public Task Delete(Guid id);
}