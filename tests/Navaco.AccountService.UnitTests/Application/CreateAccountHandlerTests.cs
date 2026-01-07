using Microsoft.Extensions.Logging;
using Navaco.AccountService.Application.Commands.CreateAccount;

namespace Navaco.AccountService.UnitTests.Application;

/// <summary>
/// تست‌های واحد برای Handler ایجاد حساب
/// </summary>
public class CreateAccountHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateAccountHandler>> _loggerMock;
    private readonly CreateAccountHandler _handler;

    public CreateAccountHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateAccountHandler>>();

        _handler = new CreateAccountHandler(
            _accountRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateAccountAndReturnId()
    {
        // Arrange
        var command = new CreateAccountCommand(
            CustomerId: Guid.NewGuid(),
            InitialBalance: 1000000,
            Currency: "IRR");

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        _accountRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithEmptyCustomerId_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateAccountCommand(
            CustomerId: Guid.Empty,
            InitialBalance: 1000000,
            Currency: "IRR");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Domain.Error");
    }

    [Fact]
    public async Task Handle_WithNegativeBalance_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateAccountCommand(
            CustomerId: Guid.NewGuid(),
            InitialBalance: -1000,
            Currency: "IRR");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Domain.Error");
    }
}
