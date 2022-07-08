using ShoppingLists.Dtos;
using ShoppingLists.Entities;

namespace ShoppingLists.Logic.ShoppingLists;

public interface IShoppingListsService
{
    public Task<List<ShoppingList>> GetByUserId(string userId);

    public Task<ShoppingList> GetWithItems(Guid id);

    public Task Create(CreateShoppingListDto dto);

    public Task Update(UpdateShoppingListDto dto);

    public Task Delete(Guid id);
}