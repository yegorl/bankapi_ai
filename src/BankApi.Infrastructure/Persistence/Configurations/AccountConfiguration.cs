using BankApi.Domain.Aggregates.Accounts;
using BankApi.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankApi.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AccountHolderId)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        // Value Objects
        builder.OwnsOne(x => x.AccountNumber, accountNumber =>
        {
            accountNumber.Property(a => a.Value)
                .HasColumnName("AccountNumber")
                .IsRequired()
                .HasMaxLength(20);
        });

        builder.OwnsOne(x => x.Balance, balance =>
        {
            balance.Property(b => b.Amount)
                .HasColumnName("Balance")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            balance.Property(b => b.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Ignore(x => x.DomainEvents);
    }
}
