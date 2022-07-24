namespace ShoppingLists.Dtos;

public class CreateListItemDto
{
    public string Name { get; set; }
    
    public int? Price { get; set; }
    
    public int Count { get; set; }
    
    public Guid ShoppingListId { get; set; }
}