using System.Data.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Minitwit.Infrastructure;

namespace Minitwit.Tests;

public class MinitwitApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{

    public MinitwitApplicationFactory()
    {
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        builder.UseEnvironment("Development");
    }
}
