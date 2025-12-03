using BankApi.Domain.Aggregates.Cards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankApi.Infrastructure.Persistence.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("Cards");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AccountId)
            .IsRequired();

        builder.Property(x => x.CardHolderName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ExpirationDate)
            .IsRequired();

        builder.Property(x => x.CVVHash)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.IsBlocked)
            .IsRequired();

        builder.Property(x => x.IsTemporarilyBlocked)
            .IsRequired();

        builder.Property(x => x.CardType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        // Value Objects
        builder.OwnsOne(x => x.CardNumber, cardNumber =>
        {
            cardNumber.Property(c => c.Value)
                .HasColumnName("CardNumber")
                .IsRequired()
                .HasMaxLength(16);
        });

        builder.Ignore(x => x.DomainEvents);
    }
}
