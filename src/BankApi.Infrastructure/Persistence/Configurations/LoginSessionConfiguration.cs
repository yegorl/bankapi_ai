using BankApi.Domain.Aggregates.LoginSessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankApi.Infrastructure.Persistence.Configurations;

public class LoginSessionConfiguration : IEntityTypeConfiguration<LoginSession>
{
    public void Configure(EntityTypeBuilder<LoginSession> builder)
    {
        builder.ToTable("LoginSessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.IpAddress)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.UserAgentSnapshot)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Success)
            .IsRequired();

        builder.Property(x => x.FailureReason)
            .HasMaxLength(500);

        builder.Property(x => x.AttemptedAt)
            .IsRequired();

        builder.Property(x => x.ExpiresAt);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.AttemptedAt);
    }
}
