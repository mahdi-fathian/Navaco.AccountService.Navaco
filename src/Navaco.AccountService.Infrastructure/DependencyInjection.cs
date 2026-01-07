using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Navaco.AccountService.Application.Common.Interfaces;
using Navaco.AccountService.Infrastructure.Persistence;
using Navaco.AccountService.Infrastructure.Persistence.Repositories;
using Navaco.AccountService.Infrastructure.Services;

namespace Navaco.AccountService.Infrastructure;

/// <summary>
/// ثبت سرویس‌های لایه Infrastructure
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AccountDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            }));

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AccountDbContext>());
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
