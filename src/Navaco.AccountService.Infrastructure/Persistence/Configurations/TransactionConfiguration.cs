namespace Navaco.AccountService.Infrastructure.Persistence.Configurations;

/// <summary>
/// پیکربندی EF Core برای Entity تراکنش
/// </summary>
public sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.AccountId)
            .IsRequired();

        builder.OwnsOne(t => t.Amount, amountBuilder =>
        {
            amountBuilder.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            amountBuilder.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(t => t.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.HasIndex(t => t.AccountId);
        builder.HasIndex(t => t.CreatedAt);
    }
}
