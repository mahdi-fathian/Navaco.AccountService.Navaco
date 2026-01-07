using Microsoft.Extensions.Logging;
using Navaco.AccountService.Application.Commands.CloseAccount;

namespace Navaco.AccountService.UnitTests.Application;

/// <summary>
/// تست‌های واحد برای Handler بستن حساب
/// </summary>
public class CloseAccountHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CloseAccountHandler>> _loggerMock;
    private readonly CloseAccountHandler _handler;

    public CloseAccountHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CloseAccountHandler>>();

        _handler = new CloseAccountHandler(
            _accountRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCloseAccountAndReturnSuccess()
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

        var command = new CloseAccountCommand(accountId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        account.Status.Should().Be(AccountStatus.Closed);

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

        var command = new CloseAccountCommand(accountId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotFound");
    }

    [Fact]
    public async Task Handle_WhenAccountAlreadyClosed_ShouldReturnFailure()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new Account(Guid.NewGuid(), new Money(1000000, "IRR"));
        account.Close();

        typeof(Entity).GetProperty("Id")!.SetValue(account, accountId);

        _accountRepositoryMock
            .Setup(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var command = new CloseAccountCommand(accountId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Domain.Error");
    }
}
