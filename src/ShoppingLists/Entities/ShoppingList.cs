namespace ShoppingLists.Entities;

public class ShoppingList
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public DateTime CreationTime { get; set; }
    
    public string UserId { get; set; }

    public List<ListItem> ListItems { get; set; }
}