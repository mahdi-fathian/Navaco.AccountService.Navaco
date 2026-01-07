using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Navaco.AccountService.Infrastructure.Persistence;

namespace Navaco.AccountService.IntegrationTests;

/// <summary>
/// Factory سفارشی برای تست‌های یکپارچه
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"TestDatabase_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // حذف DbContext واقعی
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AccountDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // حذف DbContext registration قبلی
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(AccountDbContext));

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // اضافه کردن InMemory Database با نام ثابت برای هر instance
            services.AddDbContext<AccountDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            // ساخت Service Provider و اطمینان از ایجاد دیتابیس
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
            db.Database.EnsureCreated();
        });

        builder.UseEnvironment("Testing");
    }
}
