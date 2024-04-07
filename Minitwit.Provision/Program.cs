using System.Collections.Generic;
using System.Linq;
using Pulumi;
using Minitwit.Provision.Infrastructure;
using Minitwit.Provision.Infrastructure.DigitalOcean;
using Pulumi.DigitalOcean;
using Minitwit.Provision.IO;
using Minitwit.Provision;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

const string VM_IMAGE = "ubuntu-23-10-x64";
const string REGION = "fra1";

return await Deployment.RunAsync(() =>
{
    SshKey andreasSSHKey = new("Andreas-ssh-key", new SshKeyArgs
    {
        Name = "Andreas-ssh-key",
        PublicKey = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQDHvA4lcDVXYhBWtbMu5omBwhegEAd10WBpWdvfKm/AGtM8hAosVufssiG9LZCFa+vebjDgNI7UyirIjxf6xdFqixLwTPetSMXBqvo6KhQpWwA6AoPDDpuv1wNUC292dsc1EzMVXrpjERzcAPILBsZw92Ede7OU+FI9jxf0+bzNDKrIM44CtZwRbJCtNtVzpWRZWGxv8IRT/0GQ4y4HkgG4sB9x/Y3+7m/XlUe5zzpjDNiJsw7Qo/Xkz+MNqxjiR6jdWX4FWJyPMsJW5NovrVoeUeZEq+hTMs7ytdygMRJdhiZtoSM3e8e6uExUlPv/WJOlJcWabGVjDyiB615iUE6qHVss3ya/i8hbE4Waw4R2h+O9fEQiNpywWOurrvnC54HZF2Fy3XsW8MMlcWNfsOv2v5muU5v0taTtG87NU1atCwTy6C2kyIP9UE8fWuLruWMEvz3N771cwP6YUxVJuScM9kzziMd869UZFA1xNQ3FzEOJYLnnRZ/JBH9Np2pfvtxlHM+AU2Q1OQh2QmTjDtbzpkIJoLZQ1jbGCMY3xMpCSo/VqeX2VoQk+Sp4Oo68Ae705Mpw6KX7b6g+Vy6yFKZAbLQAelCr/STYwtfnDsiUf6gjQDatmgE31UjIxzY1SFvh0sfV79vRn5utyyLhrSWevvdrRTqfP+Xa/1YSb3uBkQ== andreastietgen@Andreass-MacBook-Pro.local"
    });
    SshKey ansibleSSHKey = new("Ansible", new SshKeyArgs
    {
        Name = "Ansible",
        PublicKey = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQDHvA4lcDVXYhBWtbMu5omBwhegEAd10WBpWdvfKm/AGtM8hAosVufssiG9LZCFa+vebjDgNI7UyirIjxf6xdFqixLwTPetSMXBqvo6KhQpWwA6AoPDDpuv1wNUC292dsc1EzMVXrpjERzcAPILBsZw92Ede7OU+FI9jxf0+bzNDKrIM44CtZwRbJCtNtVzpWRZWGxv8IRT/0GQ4y4HkgG4sB9x/Y3+7m/XlUe5zzpjDNiJsw7Qo/Xkz+MNqxjiR6jdWX4FWJyPMsJW5NovrVoeUeZEq+hTMs7ytdygMRJdhiZtoSM3e8e6uExUlPv/WJOlJcWabGVjDyiB615iUE6qHVss3ya/i8hbE4Waw4R2h+O9fEQiNpywWOurrvnC54HZF2Fy3XsW8MMlcWNfsOv2v5muU5v0taTtG87NU1atCwTy6C2kyIP9UE8fWuLruWMEvz3N771cwP6YUxVJuScM9kzziMd869UZFA1xNQ3FzEOJYLnnRZ/JBH9Np2pfvtxlHM+AU2Q1OQh2QmTjDtbzpkIJoLZQ1jbGCMY3xMpCSo/VqeX2VoQk+Sp4Oo68Ae705Mpw6KX7b6g+Vy6yFKZAbLQAelCr/STYwtfnDsiUf6gjQDatmgE31UjIxzY1SFvh0sfV79vRn5utyyLhrSWevvdrRTqfP+Xa/1YSb3uBkQ== andreastietgen@Andreass-MacBook-Pro.local"
    });

    var sshKeys = new InputList<string>
    {
        andreasSSHKey.Id,
        ansibleSSHKey.Id
    };
    // Create the web servers
    IEnumerable<IVirtualMachine> webServers = DOVirtualMachine.CreateVMSet("minitwit-web-test",
                                                                            VM_IMAGE,
                                                                            REGION,
                                                                            count: 1,
                                                                            sshKeys);

    // Create the monitoring servers
    IVirtualMachine monitoringServer = DOVirtualMachine.CreateVM("minitwit-mon-test-1",
                                                                    VM_IMAGE,
                                                                    REGION,
                                                                   sshKeys);

    // Create databasecluster with minitwit database
    IDatabaseCluster minitwitDbCluster = DODatabaseCluster.CreateDatabaseCluster("minitwit-test-db-01",
                                                                                ComputeSizes.Small,
                                                                                DatabaseProviders.Postgres,
                                                                                nodecount: 1);
    minitwitDbCluster.CreateDatabase("minitwit");


    // Export outputs here
    return new Dictionary<string, object?>
    {
        ["web"] = webServers.Select(x => x.NetworkInterfaces["PublicIP"]).ToList(),
        ["monitoring"] = new List<Output<string>>() { monitoringServer.NetworkInterfaces["PublicIP"] },
    };
});
