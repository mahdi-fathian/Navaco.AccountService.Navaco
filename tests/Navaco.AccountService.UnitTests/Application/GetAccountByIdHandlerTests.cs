using Microsoft.Extensions.Logging;
using Navaco.AccountService.Application.Queries.GetAccountById;

namespace Navaco.AccountService.UnitTests.Application;

/// <summary>
/// تست‌های واحد برای Handler دریافت حساب
/// </summary>
public class GetAccountByIdHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ILogger<GetAccountByIdHandler>> _loggerMock;
    private readonly GetAccountByIdHandler _handler;

    public GetAccountByIdHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _loggerMock = new Mock<ILogger<GetAccountByIdHandler>>();

        _handler = new GetAccountByIdHandler(
            _accountRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAccountExists_ShouldReturnAccountDto()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var account = new Account(customerId, new Money(1000000, "IRR"));

        typeof(Entity).GetProperty("Id")!.SetValue(account, accountId);

        _accountRepositoryMock
            .Setup(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var query = new GetAccountByIdQuery(accountId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(accountId);
        result.Value.CustomerId.Should().Be(customerId);
        result.Value.Balance.Should().Be(1000000);
        result.Value.Currency.Should().Be("IRR");
        result.Value.Status.Should().Be("Active");
    }

    [Fact]
    public async Task Handle_WhenAccountNotFound_ShouldReturnFailure()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        _accountRepositoryMock
            .Setup(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);

        var query = new GetAccountByIdQuery(accountId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotFound");
    }
}
