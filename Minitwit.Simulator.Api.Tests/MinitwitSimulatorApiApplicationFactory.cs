using System.Data.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Minitwit.Infrastructure;
using MinitwitSimulatorAPI;

namespace Minitwit.Simulator.Api.Tests;

public class MinitwitSimulatorApiApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IDisposable where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
    }

    public override ValueTask DisposeAsync()
    {

        using (var cleanupScope = this.Services.CreateScope())
        {
            var context = cleanupScope.ServiceProvider.GetService<MinitwitContext>();

            if (context is null)
                return ValueTask.CompletedTask;

            var commandIds = new List<int> { 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010, 1011, 1012, 1013, 1014, 1015 };
            var latestCommand = context.Latests.Where(x => commandIds.Contains(x.CommandId));

            if (latestCommand is not null)
                context.Latests.RemoveRange(latestCommand);

            var testMessages = context.Messages;
            context.Messages.RemoveRange(testMessages);

            var testFollowers = context.Followers;

            context.Followers.RemoveRange(testFollowers);

            context.Users.RemoveRange(context.Users);
            context.SaveChanges();
        }

        return base.DisposeAsync();
    }
}
