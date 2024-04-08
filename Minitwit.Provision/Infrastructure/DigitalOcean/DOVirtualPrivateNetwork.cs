using Pulumi;
using Pulumi.DigitalOcean;

namespace Minitwit.Provision.Infrastructure.DigitalOcean;

internal record DOVirtualPrivateNetwork : IPrivateNetwork<DOVirtualPrivateNetwork>
{
    public Output<string> Id => _vpc.Id;

    public Output<string> Name => _vpc.Name;

    public Output<string> IPRange => _vpc.IpRange;

    private readonly Vpc _vpc;

    private DOVirtualPrivateNetwork(string name, string ipRange, string region)
    {
        _vpc = new Vpc(name, new VpcArgs
        {
            Name = name,
            IpRange = ipRange,
            Region = region
        });
    }

    public static IPrivateNetwork<DOVirtualPrivateNetwork> CreatePrivateNetwork(string name, string ipRange, string region)
        => new DOVirtualPrivateNetwork(name, ipRange, region);
}
