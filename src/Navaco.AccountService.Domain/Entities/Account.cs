namespace Navaco.AccountService.Domain.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.

public class Account : Entity
{
    private readonly List<Transaction> _transactions = new();

    public Guid CustomerId { get; private set; }
    public Money Balance { get; private set; }
    public AccountStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Account() { } // EF Core

    public Account(Guid customerId, Money initialBalance)
    {
        if (customerId == Guid.Empty)
            throw new InvalidCustomerIdException();

        Id = Guid.NewGuid();
        CustomerId = customerId;
        Balance = initialBalance ?? throw new ArgumentNullException(nameof(initialBalance));
        Status = AccountStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    public void Deposit(Money amount)
    {
        EnsureAccountIsActive();

        if (amount.Amount <= 0)
            throw new InvalidDepositAmountException(amount.Amount);

        Balance = new Money(Balance.Amount + amount.Amount, Balance.Currency);
        _transactions.Add(new Transaction(Id, amount, TransactionType.Deposit));
    }

    public void Withdraw(Money amount)
    {
        EnsureAccountIsActive();

        if (amount.Amount <= 0)
            throw new InvalidWithdrawAmountException(amount.Amount);

        if (Balance.Amount < amount.Amount)
            throw new InsufficientBalanceException(Balance.Amount, amount.Amount);

        Balance = new Money(Balance.Amount - amount.Amount, Balance.Currency);
        _transactions.Add(new Transaction(Id, amount, TransactionType.Withdrawal));
    }

    public void Close()
    {
        if (Status == AccountStatus.Closed)
            throw new AccountAlreadyClosedException(Id);

        Status = AccountStatus.Closed;
    }

    private void EnsureAccountIsActive()
    {
        if (Status != AccountStatus.Active)
            throw new AccountNotActiveException(Id);
    }
}
