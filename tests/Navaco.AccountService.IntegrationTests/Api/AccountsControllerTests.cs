using Navaco.AccountService.Api.Dtos.Requests;

namespace Navaco.AccountService.IntegrationTests.Api;

/// <summary>
/// تست‌های یکپارچه برای کنترلر حساب‌ها
/// </summary>
public class AccountsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AccountsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateAccount_WithValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var request = new CreateAccountRequest(
            CustomerId: Guid.NewGuid(),
            InitialBalance: 1000000,
            Currency: "IRR");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/accounts", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetById_WhenAccountExists_ShouldReturnOk()
    {
        // Arrange - ابتدا یک حساب ایجاد می‌کنیم
        var createRequest = new CreateAccountRequest(
            CustomerId: Guid.NewGuid(),
            InitialBalance: 1000000,
            Currency: "IRR");

        var createResponse = await _client.PostAsJsonAsync("/api/v1/accounts", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData>();
        var accountId = createResult!.Data;

        // Act
        var response = await _client.GetAsync($"/api/v1/accounts/{accountId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_WhenAccountNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/accounts/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByCustomerId_ShouldReturnOk()
    {
        // Arrange - ابتدا یک حساب ایجاد می‌کنیم
        var customerId = Guid.NewGuid();
        var createRequest = new CreateAccountRequest(
            CustomerId: customerId,
            InitialBalance: 1000000,
            Currency: "IRR");

        await _client.PostAsJsonAsync("/api/v1/accounts", createRequest);

        // Act
        var response = await _client.GetAsync($"/api/v1/accounts/customer/{customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Deposit_WithValidRequest_ShouldReturnOk()
    {
        // Arrange - ابتدا یک حساب ایجاد می‌کنیم
        var createRequest = new CreateAccountRequest(
            CustomerId: Guid.NewGuid(),
            InitialBalance: 1000000,
            Currency: "IRR");

        var createResponse = await _client.PostAsJsonAsync("/api/v1/accounts", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData>();
        var accountId = createResult!.Data;

        var depositRequest = new DepositRequest(Amount: 500000, Currency: "IRR");

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/accounts/{accountId}/deposit", depositRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Withdraw_WithValidRequest_ShouldReturnOk()
    {
        // Arrange - ابتدا یک حساب ایجاد می‌کنیم
        var createRequest = new CreateAccountRequest(
            CustomerId: Guid.NewGuid(),
            InitialBalance: 1000000,
            Currency: "IRR");

        var createResponse = await _client.PostAsJsonAsync("/api/v1/accounts", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData>();
        var accountId = createResult!.Data;

        var withdrawRequest = new WithdrawRequest(Amount: 300000, Currency: "IRR");

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/accounts/{accountId}/withdraw", withdrawRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Withdraw_WithInsufficientBalance_ShouldReturnUnprocessableEntity()
    {
        // Arrange - ابتدا یک حساب ایجاد می‌کنیم
        var createRequest = new CreateAccountRequest(
            CustomerId: Guid.NewGuid(),
            InitialBalance: 1000000,
            Currency: "IRR");

        var createResponse = await _client.PostAsJsonAsync("/api/v1/accounts", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData>();
        var accountId = createResult!.Data;

        var withdrawRequest = new WithdrawRequest(Amount: 5000000, Currency: "IRR");

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/accounts/{accountId}/withdraw", withdrawRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CloseAccount_WithValidRequest_ShouldReturnOk()
    {
        // Arrange - ابتدا یک حساب ایجاد می‌کنیم
        var createRequest = new CreateAccountRequest(
            CustomerId: Guid.NewGuid(),
            InitialBalance: 1000000,
            Currency: "IRR");

        var createResponse = await _client.PostAsJsonAsync("/api/v1/accounts", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData>();
        var accountId = createResult!.Data;

        // Act
        var response = await _client.PostAsync($"/api/v1/accounts/{accountId}/close", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CloseAccount_WhenAlreadyClosed_ShouldReturnUnprocessableEntity()
    {
        // Arrange - ابتدا یک حساب ایجاد و بسته می‌کنیم
        var createRequest = new CreateAccountRequest(
            CustomerId: Guid.NewGuid(),
            InitialBalance: 1000000,
            Currency: "IRR");

        var createResponse = await _client.PostAsJsonAsync("/api/v1/accounts", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData>();
        var accountId = createResult!.Data;

        await _client.PostAsync($"/api/v1/accounts/{accountId}/close", null);

        // Act - تلاش برای بستن مجدد
        var response = await _client.PostAsync($"/api/v1/accounts/{accountId}/close", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task HealthCheck_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // کلاس کمکی برای deserialize کردن پاسخ
    private record ApiResponseWithData(bool IsSuccess, Guid Data, string? ErrorCode, string? ErrorMessage, string TraceId, DateTime Timestamp);
}
