namespace Navaco.AccountService.Infrastructure.Persistence;

/// <summary>
/// DbContext اصلی سرویس حساب
/// </summary>
public sealed class AccountDbContext : DbContext, IUnitOfWork
{
    public AccountDbContext(DbContextOptions<AccountDbContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
