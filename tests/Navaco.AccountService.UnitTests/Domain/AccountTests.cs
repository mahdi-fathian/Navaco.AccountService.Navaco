namespace Navaco.AccountService.UnitTests.Domain;

/// <summary>
/// تست‌های واحد برای Entity حساب
/// </summary>
public class AccountTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateAccount()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var initialBalance = new Money(1000000, "IRR");

        // Act
        var account = new Account(customerId, initialBalance);

        // Assert
        account.Id.Should().NotBeEmpty();
        account.CustomerId.Should().Be(customerId);
        account.Balance.Amount.Should().Be(1000000);
        account.Balance.Currency.Should().Be("IRR");
        account.Status.Should().Be(AccountStatus.Active);
        account.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithEmptyCustomerId_ShouldThrowInvalidCustomerIdException()
    {
        // Arrange
        var customerId = Guid.Empty;
        var initialBalance = new Money(1000000, "IRR");

        // Act
        var act = () => new Account(customerId, initialBalance);

        // Assert
        act.Should().Throw<InvalidCustomerIdException>();
    }

    [Fact]
    public void Constructor_WithNullBalance_ShouldThrowArgumentNullException()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var act = () => new Account(customerId, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region Deposit Tests

    [Fact]
    public void Deposit_WithValidAmount_ShouldIncreaseBalance()
    {
        // Arrange
        var account = CreateActiveAccount(1000000);
        var depositAmount = new Money(500000, "IRR");

        // Act
        account.Deposit(depositAmount);

        // Assert
        account.Balance.Amount.Should().Be(1500000);
        account.Transactions.Should().HaveCount(1);
        account.Transactions.First().Type.Should().Be(TransactionType.Deposit);
    }

    [Fact]
    public void Deposit_WithZeroAmount_ShouldThrowInvalidDepositAmountException()
    {
        // Arrange
        var account = CreateActiveAccount(1000000);
        var depositAmount = new Money(0, "IRR");

        // Act
        var act = () => account.Deposit(depositAmount);

        // Assert
        act.Should().Throw<InvalidDepositAmountException>();
    }

    [Fact]
    public void Deposit_WhenAccountIsClosed_ShouldThrowAccountNotActiveException()
    {
        // Arrange
        var account = CreateActiveAccount(1000000);
        account.Close();
        var depositAmount = new Money(500000, "IRR");

        // Act
        var act = () => account.Deposit(depositAmount);

        // Assert
        act.Should().Throw<AccountNotActiveException>();
    }

    #endregion

    #region Withdraw Tests

    [Fact]
    public void Withdraw_WithValidAmount_ShouldDecreaseBalance()
    {
        // Arrange
        var account = CreateActiveAccount(1000000);
        var withdrawAmount = new Money(300000, "IRR");

        // Act
        account.Withdraw(withdrawAmount);

        // Assert
        account.Balance.Amount.Should().Be(700000);
        account.Transactions.Should().HaveCount(1);
        account.Transactions.First().Type.Should().Be(TransactionType.Withdrawal);
    }

    [Fact]
    public void Withdraw_WithAmountGreaterThanBalance_ShouldThrowInsufficientBalanceException()
    {
        // Arrange
        var account = CreateActiveAccount(1000000);
        var withdrawAmount = new Money(2000000, "IRR");

        // Act
        var act = () => account.Withdraw(withdrawAmount);

        // Assert
        act.Should().Throw<InsufficientBalanceException>();
    }

    [Fact]
    public void Withdraw_WithZeroAmount_ShouldThrowInvalidWithdrawAmountException()
    {
        // Arrange
        var account = CreateActiveAccount(1000000);
        var withdrawAmount = new Money(0, "IRR");

        // Act
        var act = () => account.Withdraw(withdrawAmount);

        // Assert
        act.Should().Throw<InvalidWithdrawAmountException>();
    }

    [Fact]
    public void Withdraw_WhenAccountIsClosed_ShouldThrowAccountNotActiveException()
    {
        // Arrange
        var account = CreateActiveAccount(1000000);
        account.Close();
        var withdrawAmount = new Money(300000, "IRR");

        // Act
        var act = () => account.Withdraw(withdrawAmount);

        // Assert
        act.Should().Throw<AccountNotActiveException>();
    }

    #endregion

    #region Close Tests

    [Fact]
    public void Close_WhenAccountIsActive_ShouldChangeStatusToClosed()
    {
        // Arrange
        var account = CreateActiveAccount(1000000);

        // Act
        account.Close();

        // Assert
        account.Status.Should().Be(AccountStatus.Closed);
    }

    [Fact]
    public void Close_WhenAccountIsAlreadyClosed_ShouldThrowAccountAlreadyClosedException()
    {
        // Arrange
        var account = CreateActiveAccount(1000000);
        account.Close();

        // Act
        var act = () => account.Close();

        // Assert
        act.Should().Throw<AccountAlreadyClosedException>();
    }

    #endregion

    #region Helper Methods

    private static Account CreateActiveAccount(decimal balance)
    {
        return new Account(Guid.NewGuid(), new Money(balance, "IRR"));
    }

    #endregion
}
