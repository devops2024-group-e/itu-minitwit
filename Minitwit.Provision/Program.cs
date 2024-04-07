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
        PublicKey = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQDUoPbebMqsdvUWEDHE5L7VA7mU51rwyWhJFHQ+r0JYOPe5MYHZrQzckIZkslQaWF7yt5CvQySkG9iVJ06vN0sQoUmolua8ys87pYmqgXukx3W37UF/V7dLKopgpEUny1MWYlj+C3g0uSeMYGNLyNP6OnSLRWvD9qKiTThVZFOn9MCwMFBP90jizwvQDrAkzRvIrtFe/L3In50wuk8YYrpZE0b11zt3gyywKLf+ajc1lZOfM3c1MDAA4nqrVFDvoeCyFeu9KqfOvEbGqGNfIgGeIu7dKKBUHt0I36CQAkDvSz06yf3us+0zeOz0yJnKPfZidHk+yGeR/LyMOHHOqx+pPIKgP5yggs+FuuMaWjQzzUyvVcN/U3mxnFNIFX8vmI7JWcpJCCgeMog+10jQxmlPq1GvpS0my+g+7/SmKv5+zFHTMcSdHnhyaf3B1gSoJOYr47KqLBp3Qt7t9LxBQ4We3uISrOT9Q5XBVPk51PKi8vsOMbmqL4JZILzB0dwKq0M52nM/a1Gb1Oo7B/h72D4S1++7vHQSrrnW86pRQ8Izwg01xfvJc7FTHBsbO6jQAtfek9x/CNacp8YIt8WwyljoQBlY5+0VieEUKFY7pjUBF4yw5osEjRp620dx4GstnIBwb5AZlq5JBmCevDmiFqFm7sAP+1NoFhEXbFIZffV0Uw== andreastietgen@andreass-mbp.lan"
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
