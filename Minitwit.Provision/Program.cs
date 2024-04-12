using System.Collections.Generic;
using System.Linq;
using Pulumi;
using Minitwit.Provision.Infrastructure;
using Minitwit.Provision.Infrastructure.DigitalOcean;
using Pulumi.DigitalOcean;
using Minitwit.Provision;

const string VM_IMAGE = "ubuntu-23-10-x64";
const string REGION = "fra1";

return await Deployment.RunAsync(() =>
{
    var sshKeys = CreateSSHKeys();

    // Has IP range from 192.168.1.1->192.168.1.254, MASK is 255.255.255.0
    var minitwitVPC = DOVirtualPrivateNetwork.CreatePrivateNetwork("minitwit-prod-vpc", "192.168.1.0/24", REGION);

    // Create the web servers
    IEnumerable<IVirtualMachine> swmNodes = DOVirtualMachine.CreateVMSet("minitwit-swm-node",
                                                                            VM_IMAGE,
                                                                            minitwitVPC.Id,
                                                                            REGION,
                                                                            count: 2,
                                                                            sshKeys);

    IEnumerable<IVirtualMachine> swmManagers = DOVirtualMachine.CreateVMSet("minitwit-swm-man",
                                                                            VM_IMAGE,
                                                                            minitwitVPC.Id,
                                                                            REGION,
                                                                            count: 1,
                                                                            sshKeys);


    //Create the monitoring servers
    // IVirtualMachine monitoringServer = DOVirtualMachine.CreateVM("minitwit-mon-1",
    //                                                                 VM_IMAGE,
    //                                                                 minitwitVPC.Id,
    //                                                                 REGION,
    //                                                                sshKeys);

    // Create databasecluster with minitwit database
    // IDatabaseCluster minitwitDbCluster = DODatabaseCluster.CreateDatabaseCluster("minitwit-test-db-01",
    //                                                                             ComputeSizes.Small,
    //                                                                             DatabaseProviders.Postgres,
    //                                                                             minitwitVPC.Id,
    //                                                                             nodecount: 1);
    // minitwitDbCluster.CreateDatabase("minitwit");



    var minitwitProject = new Project("Minitwit", new ProjectArgs()
    {
        Name = "Minitwit",
        Description = "A mini Twitter clone application for the course DevOps at the IT University of Copenhagen",
        Purpose = "Application Environment",
        Resources = new InputList<string>
        {
            swmManagers.Select(x => x.Name).ToArray(),
            swmNodes.Select(x => x.Name).ToArray(),
            // monitoringServer.Name,
            // minitwitDbCluster.Name,
        }
    });


    // Export outputs here
    return new Dictionary<string, object?>
    {
        ["swmmanagers"] = swmManagers.Select(x => x.NetworkInterfaces["PublicIP"]).ToList(),
        ["swmnodes"] = swmNodes.Select(x => x.NetworkInterfaces["PublicIP"]).ToList(),
        // ["monitoring"] = new List<Output<string>>() { monitoringServer.NetworkInterfaces["PublicIP"] },
    };
});

static InputList<string> CreateSSHKeys()
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
    SshKey malinSSHKey = new("Malin-ssh-key", new SshKeyArgs
    {
        Name = "Malin-ssh-key",
        PublicKey = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQDImN+79Poq8zUQTCfS+sIKJwnJfLihtWQgFrb96+TFKtYCq89k/2ZBeYTjZm/fHctN3MrGdpaz4xUWnAmdJqRxtgQjx2owPA0TW68tjKYLTRaKltmQfaTxCa38gjwUw3NnvcZcpNDzjVwD4zXF1OuQfMPdj63ZK9O5EUSNT4JhqpyS8J1wjEUQ1kddtgOt2Wbm1lm/LMbadzCDpof0J7FlT4IGJzyDLU8JUWOdFitGEo1oruaCZF6ycIpDzQxNYh7mwhRIdIObi/d0S4eCnwUcHSTJiDcKiZ4SADcrKIeMpy9/aDaIhH5cprPXuT484UwX6hzTIwkOVnuE5Urq5IwBCVtJtUXv1tgw1Un8qv72mka3jroIPLKlq46do90eR/y9TiZlUfX9JeF1ejcz4DV8zQxhsfPeDBbJdI8CpGkqrA8rTCdqg7p1FhHCw9wF3zyAXmj4ppnPVLQu6txDtDyel54Vzidu8KT4lqeH2XGLcd8sfwHIheCXWDUmdyNMUg0hlhJcclvUIej6T0MT/8Zq/4uovP3f9pr1uVkaHP5sKqjFxHAn9OmFHTCO/5MwsCVErfWPpaT2KQvDU/7OUKUrR7R2SKE/oOgcaJbEpPaKCRTskMFfdru/pLO/wAjvV3xAOqxR/B0upA/Rk54D5nhAt3T0fBUTwT/zkl03kBdqTw== malin nordqvist@DESKTOP-90ST137"
    });
    SshKey sarahSSHKey = new("Sarah-ssh-key", new SshKeyArgs
    {
        Name = "Sarah-ssh-key",
        PublicKey = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQD6XFydqlxARlR/NNhVKN6aqoq6ew5pClNPCp90X2R07q9k/BOJH1LEvh5agq1zyCWMdJl1/EISUmxWGawDmAp+BA+Sye3jHt80h1g/Z2bofUJZmCWsngk5dBQRuxff1tFHwjEgyuVcEfx558PKN/qSAIeoLGjOb08gCCOXTggP3j4E7/K4LBaJpOuYZOWxXFAXSMjDVA9seRKAOFlO/qw5hLYCJGlALGGbmhXPYyH5qDEdp3dX8iQEJVrF/WV+gcnl8WJHGw0bERTLWr4OERmfZP+xL5XjjTzUILQvAKx7KMNFbJ9VdjdjLUS14+uJmLfz538NUuHl2bgrVC68FOHF6G9HW9SgRW55VL59h/43YbvlHGIHYZXcvR1DZDkhOm7gN60bQpMP5w2mpygP8EdYD7ydp9gzQRM5F1CDBNMO4GpU+LT3ycUZDqlZyZsA5j/bMUwskgBGvGuHescbAlw9hYHwZds698vSxpoonrGcgXceeO2DECSgViBeXOJyTMAtNMhXhK1vrxz0jwmAqnXty28jmdxTBdceBSVkFvit4dynqyAG6IKtXWQPg0eU5PWiRplez8mlSF3ByoXjXeGAR/5YZRVRhN97Z94/v8T2/7+BNqCIX2/l1MrU9XqH0KvR5vIG+aG7NiADWSankXHMbbDiPpKbEL7BkFqvOxIDDw== sarah@Sarah-Legion"
    });
    SshKey milleLaptopSSHKey = new("Mille-laptop-ssh-key", new SshKeyArgs
    {
        Name = "Mille-laptop-ssh-key",
        PublicKey = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABgQCMc7BPduNHlbfMWkJrvRqvrhcW53bks4zjxG/YrKMYFR/vVN/yXBNUJERENQfFed7/7BbISE/B0JbIfTBlyFQrOm63+qpUcwkK5q5W9ubPO59q00bif2/DPg5BB3l5NrYKf6komNuXMLmhHMrgZswAQeG+Vmi/ZR/cjEo1YH96raVlCSyMplEI+ha8xKUg0BDXShEaYHaR2aSrtgOJgOK1FMwpN2U6tNjeO4j6093oxJXqYi59c98ykLTKK/p30tnnD57bk42+dtgKGhy0E9qaAUZHEVapnEgbOE7dN5O48VkdNpcHZvYB3TsrdsbW313RC+AUN40/ZC37rMrb1n/96+gcC4a+TWVzyDAnno/66t8kKAShgqNNlljSU9o33MXZ0ojosyCtH9ZW8RbD2hJIRmwOqitH+CR28Kt4k4yavgS0SMQF6tQ4FaqTnP/ufOuWDuLfhmKpA9992CU6rm4mC6p9oV1ZajLbb2yhM/JsjE5AAiQOjAsv32N202DJBEk= pinkvinus@ScuffedKeyboardComputerNix"
    });
    // SshKey millePcSSHKey = new("Mille-pc-ssh-key", new SshKeyArgs
    // {
    //     Name = "Mille-pc-ssh-key",
    //     PublicKey = "ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIEZanDonPqwSD+2LkwBtk9GsZhO+nr96Yai9WEqAyQFA pinkvinus@NonScuffedComputerNix"
    // });
    // SshKey mySSHKey = new("My-ssh-key", new SshKeyArgs
    // {
    //     Name = "My-ssh-key",
    //     PublicKey = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABgQDKtSKzMmwH4wslwad2t9VAfGS8A6CKkiejZNXMGbYavZtzSSX3ZVoqFSaKW+DLWsUi8dnF1en9+c6tkiafQ0frreVKddXWU21Q6vT1oSii5+RLDzb9iIVBnajeRO/89taFJQs3jlJr703zJKz+EnY+orK2VC9JNX9r+j1U5ugvyL1LiogaWaCl+r9cM/tfN7M1j+2TOitZPp7JH2SPJv8DqdjjWKprRYAQQ+n8RMqlFRqS6Lwc686mzR6DBQlqehZc1YsbihNlQkGj3VR5Kyw0YnEbdZvW17v78NMKM5Ilsylnt30VPDNPBcbhmQuNXOVUApM73EnLXsWjEiGgZ3xkq03zdH5I6R27X5CPLr6Pa+OyYCR0EhuelFkwK/1Sv50z6enH6sbxU1L35UG6RqsM33PB4j/D5ix5wLbmFrC1bqe4Xb8C79kBffS5FYL7tETtfX1J8DRo5uBotPmNxSuZxBKHG4d7W716gmvpH6p0rgF5p2R6RgG+uYEpsjYznk8= myssenberg@Mys-MacBook-Air.local"
    // });

    return new()
    {
        andreasSSHKey.Id,
        ansibleSSHKey.Id,
        malinSSHKey.Id,
        // sarahSSHKey.Id,
        milleLaptopSSHKey.Id,
        // millePcSSHKey.Id,
        // mySSHKey.Id,
    };
}
