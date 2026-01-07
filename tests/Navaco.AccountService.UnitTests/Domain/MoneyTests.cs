namespace Navaco.AccountService.UnitTests.Domain;

/// <summary>
/// تست‌های واحد برای Value Object پول
/// </summary>
public class MoneyTests
{
    [Fact]
    public void Constructor_WithValidAmount_ShouldCreateMoney()
    {
        // Arrange & Act
        var money = new Money(1000000, "IRR");

        // Assert
        money.Amount.Should().Be(1000000);
        money.Currency.Should().Be("IRR");
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ShouldThrowInvalidMoneyAmountException()
    {
        // Arrange & Act
        var act = () => new Money(-100, "IRR");

        // Assert
        act.Should().Throw<InvalidMoneyAmountException>();
    }

    [Fact]
    public void Constructor_WithZeroAmount_ShouldCreateMoney()
    {
        // Arrange & Act
        var money = new Money(0, "IRR");

        // Assert
        money.Amount.Should().Be(0);
    }

    [Fact]
    public void Addition_WithSameCurrency_ShouldReturnCorrectSum()
    {
        // Arrange
        var money1 = new Money(1000, "IRR");
        var money2 = new Money(500, "IRR");

        // Act
        var result = money1 + money2;

        // Assert
        result.Amount.Should().Be(1500);
        result.Currency.Should().Be("IRR");
    }

    [Fact]
    public void Addition_WithDifferentCurrency_ShouldThrowCurrencyMismatchException()
    {
        // Arrange
        var money1 = new Money(1000, "IRR");
        var money2 = new Money(500, "USD");

        // Act
        var act = () => money1 + money2;

        // Assert
        act.Should().Throw<CurrencyMismatchException>();
    }

    [Fact]
    public void Subtraction_WithSameCurrency_ShouldReturnCorrectDifference()
    {
        // Arrange
        var money1 = new Money(1000, "IRR");
        var money2 = new Money(300, "IRR");

        // Act
        var result = money1 - money2;

        // Assert
        result.Amount.Should().Be(700);
    }

    [Fact]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var money1 = new Money(1000, "IRR");
        var money2 = new Money(1000, "IRR");

        // Act & Assert
        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var money1 = new Money(1000, "IRR");
        var money2 = new Money(2000, "IRR");

        // Act & Assert
        money1.Should().NotBe(money2);
        (money1 != money2).Should().BeTrue();
    }

    [Fact]
    public void Zero_ShouldReturnMoneyWithZeroAmount()
    {
        // Arrange & Act
        var money = Money.Zero("IRR");

        // Assert
        money.Amount.Should().Be(0);
        money.Currency.Should().Be("IRR");
    }
}
