using Microsoft.Extensions.Logging;
using Navaco.AccountService.Application.Commands.Withdraw;

namespace Navaco.AccountService.UnitTests.Application;

/// <summary>
/// تست‌های واحد برای Handler برداشت
/// </summary>
public class WithdrawHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<WithdrawHandler>> _loggerMock;
    private readonly WithdrawHandler _handler;

    public WithdrawHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<WithdrawHandler>>();

        _handler = new WithdrawHandler(
            _accountRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldWithdrawAndReturnSuccess()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new Account(Guid.NewGuid(), new Money(1000000, "IRR"));

        typeof(Entity).GetProperty("Id")!.SetValue(account, accountId);

        _accountRepositoryMock
            .Setup(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new WithdrawCommand(accountId, 300000, "IRR");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        account.Balance.Amount.Should().Be(700000);

        _accountRepositoryMock.Verify(
            x => x.UpdateAsync(account, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAccountNotFound_ShouldReturnFailure()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        _accountRepositoryMock
            .Setup(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);

        var command = new WithdrawCommand(accountId, 300000, "IRR");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotFound");
    }

    [Fact]
    public async Task Handle_WithInsufficientBalance_ShouldReturnFailure()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new Account(Guid.NewGuid(), new Money(100000, "IRR"));

        typeof(Entity).GetProperty("Id")!.SetValue(account, accountId);

        _accountRepositoryMock
            .Setup(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var command = new WithdrawCommand(accountId, 500000, "IRR");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Domain.Error");
    }
}
