using Microsoft.EntityFrameworkCore;
using Operations.Entities;

namespace Operations.Data;

public class OperationsContext : DbContext
{
    public DbSet<Card> Cards { get; set; }
    
    public DbSet<Payment> Payments { get; set; }
    
    public DbSet<Income> Incomes { get; set; }
    
    public OperationsContext(DbContextOptions<OperationsContext> options) : base(options) { }
}