using Common.Interfaces;
using Operations.Dtos;
using Operations.Dtos.Card;

namespace Operations.Entities;

public class Card : IEntity, IUserBelonging
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int Balance { get; set; }
    
    public string UserId { get; set; }
    
    public List<PaymentRecord> PaymentRecords { get; set; }
    
    public List<IncomeRecord> IncomeRecords { get; set; }
    
    public List<RegularIncome> RegularIncomes { get; set; }
    
    public List<RegularPayment> RegularPayments { get; set; }

    public Card(CreateCardDto dto)
    {
        Id = Guid.NewGuid();
        Name = dto.Name;
        Balance = dto.StartBalance;
        UserId = dto.UserId;
    }
    
    public Card() {}
}