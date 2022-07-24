using Microsoft.EntityFrameworkCore;
using Operations.Entities;

namespace Operations.Data;

public class OperationsContext : DbContext
{
    public DbSet<Card> Cards { get; set; }
    
    public DbSet<PaymentRecord> PaymentRecords { get; set; }
    
    public DbSet<IncomeRecord> IncomeRecords { get; set; }
    
    public DbSet<RegularPayment> RegularPayments { get; set; }
    
    public DbSet<RegularIncome> RegularIncomes { get; set; }

    public OperationsContext(DbContextOptions<OperationsContext> options) : base(options) { }
}