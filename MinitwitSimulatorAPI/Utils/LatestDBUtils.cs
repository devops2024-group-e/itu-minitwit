using Minitwit.Infrastructure.Repositories;

namespace MinitwitSimulatorAPI.Utils;

public class LatestDBUtils
{
    public static void UpdateLatest(ILatestRepository latestRepository, int CommandId)
    {   
        //NOTE: this is called "UpdateLatest" but it seems that it just adds a Latest
        latestRepository.AddLatest(CommandId);
    }
}