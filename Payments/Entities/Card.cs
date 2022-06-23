namespace Payments.Entities;

public class Card
{
    public Guid Id { get; set; }
    
    public int Balance { get; set; }
    
    public List<Payment> Payments { get; set; }
    
    public List<Income> Incomes { get; set; }

    public Card(int balance)
    {
        Id = Guid.NewGuid();
        Balance = balance;
    }
    
    private Card() {}
}