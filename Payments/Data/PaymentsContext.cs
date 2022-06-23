using Microsoft.EntityFrameworkCore;
using Payments.Entities;

namespace Payments.Data;

public class PaymentsContext : DbContext
{
    public DbSet<Card> Cards { get; set; }
    
    public DbSet<Payment> Payments { get; set; }
    
    public DbSet<Income> Incomes { get; set; }
    
    public PaymentsContext(DbContextOptions<PaymentsContext> options) : base(options) { }
}