using System.Data.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using MinitwitSimulatorAPI;

namespace Minitwit.Simulator.Api.Tests;

public class MinitwitSimulatorApiApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{

    public MinitwitSimulatorApiApplicationFactory()
    {

    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the application registered Minitwit services
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<MinitwitContext>));

            services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));

            services.Remove(dbConnectionDescriptor);

            services.AddDbContext<MinitwitContext>((container, options) =>
            {
                options.UseNpgsql($"Host=127.0.0.1;Port=5432;Username=minitwit-sa;Password=123;Database=minitwit");
            });


        });

        builder.UseEnvironment("Development");
    }
}
