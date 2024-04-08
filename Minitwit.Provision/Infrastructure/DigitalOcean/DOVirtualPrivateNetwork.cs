using Pulumi;
using Pulumi.DigitalOcean;

namespace Minitwit.Provision.Infrastructure.DigitalOcean;

internal record DOVirtualPrivateNetwork : IPrivateNetwork<DOVirtualPrivateNetwork>
{
    public Output<string> Id => _vpc.Id;

    public Output<string> Name => _vpc.Name;

    public Output<string> IPRange => _vpc.IpRange;

    private readonly Vpc _vpc;

    private DOVirtualPrivateNetwork(string name, string ipRange)
    {
        _vpc = new Vpc(name, new VpcArgs
        {
            Name = name,
            IpRange = ipRange
        });
    }

    public static IPrivateNetwork<DOVirtualPrivateNetwork> CreatePrivateNetwork(string name, string ipRange)
        => new DOVirtualPrivateNetwork(name, ipRange);
}
