using Minitwit;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Minitwit.Infrastructure;
using Minitwit.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;


var builder = WebApplication.CreateBuilder(args);

// Register source of configurations
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables(prefix: "Minitwit_");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MinitwitContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("MinitwitDatabase"));
});

// Add dependencies to dependency injection
builder.Services.AddScoped<IFollowerRepository,FollowerRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddOpenTelemetry()
  .WithMetrics(b => b.AddAspNetCoreInstrumentation()
                     .AddPrometheusExporter());

// Add session settings
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Ensure that the database is created when in development mode
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetService<MinitwitContext>();
        if (dbContext is not null)
            dbContext.Database.EnsureCreated();

    }
}

// Ensure that the database is created when in development mode
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetService<MinitwitContext>();
    if (dbContext is not null)
        dbContext.Database.EnsureCreated();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Timeline}/{action=Index}/{id?}");

app.Run();

// Adding this in order to make the Program class visible to our integration tests
// as stated by the offical documentation https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0
public partial class Program { }
