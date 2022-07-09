using Common.Interfaces;
using ShoppingLists.Dtos;

namespace ShoppingLists.Entities;

public class ListItem : IEntity
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int? Price { get; set; }
    
    public int Count { get; set; }
    
    public bool IsPurchased { get; set; }
    
    public DateTime CreationTime { get; set; }
    
    public Guid ShoppingListId { get; set; }
    
    public ShoppingList ShoppingList { get; set; }

    public ListItem(CreateListItemDto dto)
    {
        Id = Guid.NewGuid();
        Name = dto.Name;
        Price = dto.Price;
        Count = dto.Count;
        IsPurchased = false;
        CreationTime = DateTime.UtcNow;
        ShoppingListId = dto.ShoppingListId;
    }
    
    private ListItem() {}
}