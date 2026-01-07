namespace Navaco.AccountService.Infrastructure.Persistence.Configurations;

/// <summary>
/// پیکربندی EF Core برای Entity حساب
/// </summary>
public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.CustomerId)
            .IsRequired();

        builder.OwnsOne(a => a.Balance, balanceBuilder =>
        {
            balanceBuilder.Property(m => m.Amount)
                .HasColumnName("Balance")
                .HasPrecision(18, 2)
                .IsRequired();

            balanceBuilder.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.HasMany(a => a.Transactions)
            .WithOne()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.CustomerId);
    }
}
