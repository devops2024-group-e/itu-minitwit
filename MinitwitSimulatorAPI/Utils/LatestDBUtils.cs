using MinitwitSimulatorAPI.Models;

namespace MinitwitSimulatorAPI.Utils;

public class LatestDBUtils
{
    public static void UpdateLatest(MinitwitContext context, int CommandId)
    {   
        Latest latest = new Latest
        {
            CommandId = CommandId
        };

        context.Latests.Add(latest);
        context.SaveChanges();
    }
}