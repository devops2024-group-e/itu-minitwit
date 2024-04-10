using Pulumi;

namespace Minitwit.Provision.Infrastructure;

internal interface IPrivateNetwork
{
    Output<string> Id { get; }

    Output<string> Name { get; }

    Output<string> IPRange { get; }
}
