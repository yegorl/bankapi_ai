using BankApi.Domain.Aggregates.MoneyTransfers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankApi.Infrastructure.Persistence.Configurations;

public class MoneyTransferConfiguration : IEntityTypeConfiguration<MoneyTransfer>
{
    public void Configure(EntityTypeBuilder<MoneyTransfer> builder)
    {
        builder.ToTable("MoneyTransfers");

        builder.HasKey(mt => mt.Id);

        builder.Property(mt => mt.SourceCardNumber)
            .IsRequired()
            .HasMaxLength(19);

        builder.Property(mt => mt.TargetCardNumber)
            .IsRequired()
            .HasMaxLength(19);

        builder.Property(mt => mt.SourceAccountId)
            .IsRequired();

        builder.Property(mt => mt.TargetAccountId)
            .IsRequired();

        builder.OwnsOne(mt => mt.Amount, amount =>
        {
            amount.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            amount.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(mt => mt.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(mt => mt.Description)
            .HasMaxLength(500);

        builder.Property(mt => mt.FailureReason)
            .HasMaxLength(1000);

        builder.Property(mt => mt.CreatedAt)
            .IsRequired();

        builder.Property(mt => mt.UpdatedAt)
            .IsRequired();

        builder.Property(mt => mt.IsDeleted)
            .IsRequired();

        // Indexes
        builder.HasIndex(mt => mt.SourceCardNumber);
        builder.HasIndex(mt => mt.TargetCardNumber);
        builder.HasIndex(mt => mt.CreatedAt);
        builder.HasIndex(mt => mt.Status);

        // Ignore domain events
        builder.Ignore(mt => mt.DomainEvents);
    }
}
