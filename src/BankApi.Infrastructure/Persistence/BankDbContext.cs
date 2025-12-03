using BankApi.Domain.Aggregates.AccountHolders;
using BankApi.Domain.Aggregates.Accounts;
using BankApi.Domain.Aggregates.Cards;
using BankApi.Domain.Aggregates.Transactions;
using BankApi.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Infrastructure.Persistence;

/// <summary>
/// Database context for the banking application
/// </summary>
public class BankDbContext : DbContext
{
    public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<AccountHolder> AccountHolders => Set<AccountHolder>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankDbContext).Assembly);
    }
}
