using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Entities;

namespace Operations.Data.Configurations;

public class PaymentRecordConfiguration : IEntityTypeConfiguration<PaymentRecord>
{
    public void Configure(EntityTypeBuilder<PaymentRecord> builder)
    {
        builder
            .HasOne(record => record.RegularPayment)
            .WithMany(regularPayment => regularPayment.PaymentRecords)
            .HasForeignKey(record => record.RegularPaymentId);
    }
}