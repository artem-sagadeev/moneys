namespace ShoppingLists.Dtos;

public class UpdateListItemDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int? Price { get; set; }
    
    public int Count { get; set; }
    
    public Guid ListId { get; set; }
}