using ShoppingLists.Dtos;
using ShoppingLists.Entities;

namespace ShoppingLists.Logic.ListItems;

public interface IListItemService
{
    public Task<ListItem> GetById(Guid id);

    public Task<List<ListItem>> GetByShoppingListId(Guid shoppingListId);

    public Task Create(CreateListItemDto dto);

    public Task Update(UpdateListItemDto dto);

    public Task Purchase(Guid id);

    public Task CancelPurchase(Guid id);

    public Task Delete(Guid id);
}