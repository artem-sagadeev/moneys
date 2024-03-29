using Common.Interfaces;
using Operations.Dtos;

namespace Operations.Entities;

public class Card : IEntity, IUserBelonging
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public decimal Balance { get; set; }
    
    public string UserId { get; set; }
    
    public List<Payment> Payments { get; set; }
    
    public List<Income> Incomes { get; set; }

    public Card(CreateCardDto dto)
    {
        Id = Guid.NewGuid();
        Name = dto.Name;
        Balance = dto.StartBalance;
        UserId = dto.UserId;
    }
    
    private Card() {}
}