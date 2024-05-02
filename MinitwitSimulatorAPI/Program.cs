using Microsoft.EntityFrameworkCore;
using MinitwitSimulatorAPI.Models;
using MinitwitSimulatorAPI;
using Minitwit.Infrastructure.Repositories;
using Minitwit.Infrastructure;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables(prefix: "Minitwit_");

builder.Services.AddControllers();
builder.Services.AddDbContext<MinitwitContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("MinitwitDatabase"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDistributedMemoryCache();

// Add dependencies to dependency injection
builder.Services.AddScoped<ILatestRepository, LatestRepository>();
builder.Services.AddScoped<IFollowerRepository, FollowerRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddOpenTelemetry()
  .WithMetrics(b => b.AddAspNetCoreInstrumentation()
                     .AddPrometheusExporter());

var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllers();

app.Run();

// Adding this in order to make the Program class visible to our integration tests
// as stated by the offical documentation https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0
public partial class Program { }
