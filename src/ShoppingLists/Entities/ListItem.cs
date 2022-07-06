namespace ShoppingLists.Entities;

public class ListItem
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int? Price { get; set; }
    
    public int Count { get; set; }
    
    public bool IsPurchased { get; set; }
    
    public DateTime CreationTime { get; set; }
    
    public Guid ShoppingListId { get; set; }
    
    public ShoppingList ShoppingList { get; set; }
}