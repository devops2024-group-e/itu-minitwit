using Minitwit;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Minitwit.Infrastructure;
using Minitwit.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Register source of configurations
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables(prefix: "Minitwit_");

builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MinitwitContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("MinitwitDatabase"));
});

// Add session settings
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
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
