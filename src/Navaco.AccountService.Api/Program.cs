using System.Reflection;
using Microsoft.OpenApi.Models;
using Navaco.AccountService.Api.Middlewares;
using Navaco.AccountService.Application;
using Navaco.AccountService.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration - Structured Logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Navaco.AccountService")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services - Clean Architecture layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger Configuration - Full Documentation
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Navaco Account Service API",
        Version = "v1",
        Description = """
            سرویس مدیریت حساب‌های بانکی ناواکو
            
            ## قابلیت‌ها
            - ایجاد حساب جدید
            - واریز به حساب
            - برداشت از حساب
            - بستن حساب
            - مشاهده اطلاعات حساب
            - مشاهده حساب‌های مشتری
            
            ## استانداردها
            این سرویس بر اساس استانداردهای مهندسی ناواکو و معماری Clean Architecture پیاده‌سازی شده است.
            """,
        Contact = new OpenApiContact
        {
            Name = "تیم توسعه ناواکو",
            Email = "dev@navaco.ir"
        }
    });

    // Include XML comments
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Enable annotations
    options.EnableAnnotations();

    // Custom schema IDs
    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
});

var app = builder.Build();

// Configure pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Navaco Account Service v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Navaco Account Service API";
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        options.EnableDeepLinking();
        options.DisplayRequestDuration();
    });
}

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
    };
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new 
{ 
    Status = "Healthy", 
    Service = "Navaco.AccountService",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0"
}))
.WithName("HealthCheck")
.WithTags("Health")
.WithDescription("بررسی سلامت سرویس");

app.Run();

// برای تست‌های یکپارچه
public partial class Program { }
