using BankApi.Domain.Aggregates.AccountHolders;
using BankApi.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankApi.Infrastructure.Persistence.Configurations;

public class AccountHolderConfiguration : IEntityTypeConfiguration<AccountHolder>
{
    public void Configure(EntityTypeBuilder<AccountHolder> builder)
    {
        builder.ToTable("AccountHolders");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DateOfBirth)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        // Value Objects
        builder.OwnsOne(x => x.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(200);
        });

        builder.OwnsOne(x => x.Phone, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("Phone")
                .IsRequired()
                .HasMaxLength(20);
        });

        builder.OwnsOne(x => x.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("AddressStreet")
                .HasMaxLength(200);

            address.Property(a => a.City)
                .HasColumnName("AddressCity")
                .HasMaxLength(100);

            address.Property(a => a.State)
                .HasColumnName("AddressState")
                .HasMaxLength(100);

            address.Property(a => a.PostalCode)
                .HasColumnName("AddressPostalCode")
                .HasMaxLength(20);

            address.Property(a => a.Country)
                .HasColumnName("AddressCountry")
                .HasMaxLength(50);
        });

        builder.Ignore(x => x.DomainEvents);
    }
}
