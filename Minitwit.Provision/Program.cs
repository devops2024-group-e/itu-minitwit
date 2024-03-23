using System.Collections.Generic;
using System.Linq;
using Pulumi;
using Minitwit.Provision.Infrastructure;
using Minitwit.Provision.Infrastructure.DigitalOcean;
using Pulumi.DigitalOcean;

const string VM_IMAGE = "ubuntu-20-04-x64";
const string REGION = "fra1";

return await Deployment.RunAsync(() =>
{
    SshKey andreasSSHKey = new("Andreas-ssh-key", new SshKeyArgs
    {
        Name = "Andreas-ssh-key",
        PublicKey = "Abe"
    });

    // Create the web servers
    IEnumerable<IVirtualMachine> webServers = DOVirtualMachine.CreateVMSet("minitwit-web-test", VM_IMAGE, REGION, count: 3);

    // Create the monitoring servers
    IVirtualMachine monitoringServer = DOVirtualMachine.CreateVM("minitwit-mon-test-1", VM_IMAGE, REGION);

    // Export outputs here
    return new Dictionary<string, object?>
    {
        ["web-server-ips"] = webServers.Select(x => x.NetworkInterfaces["PublicIP"]).ToList(),
        ["monitoring-server-ips"] = monitoringServer.NetworkInterfaces["PublicIP"]
    };
});
