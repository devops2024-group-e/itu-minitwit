using MinitwitSimulatorAPI;

namespace Minitwit.Simulator.Api.Tests.Controllers;

[CollectionDefinition(nameof(SequentialControllerTestCollectionDefinition), DisableParallelization = true)]
public class SequentialControllerTestCollectionDefinition : ICollectionFixture<MinitwitSimulatorApiApplicationFactory<Program>>
{
}
