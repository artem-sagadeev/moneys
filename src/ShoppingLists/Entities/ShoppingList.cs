using Common.Interfaces;
using ShoppingLists.Dtos;

namespace ShoppingLists.Entities;

public class ShoppingList : IEntity, IUserBelonging
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public DateTime CreationTime { get; set; }
    
    public string UserId { get; set; }

    public List<ListItem> ListItems { get; set; }

    public ShoppingList(CreateShoppingListDto dto)
    {
        Id = Guid.NewGuid();
        Name = dto.Name;
        CreationTime = DateTime.UtcNow;
        UserId = dto.UserId;
    }
    
    private ShoppingList() {}
}