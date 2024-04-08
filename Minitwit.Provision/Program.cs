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
    IEnumerable<IVirtualMachine> webServers = DOVirtualMachine.CreateVMSet("minitwit-web-test",
                                                                            VM_IMAGE,
                                                                            minitwitVPC.Id,
                                                                            REGION,
                                                                            count: 2,
                                                                            sshKeys);

    // Create the monitoring servers
    IVirtualMachine monitoringServer = DOVirtualMachine.CreateVM("minitwit-mon-test-1",
                                                                    VM_IMAGE,
                                                                    minitwitVPC.Id,
                                                                    REGION,
                                                                   sshKeys);

    // Create databasecluster with minitwit database
    IDatabaseCluster minitwitDbCluster = DODatabaseCluster.CreateDatabaseCluster("minitwit-test-db-01",
                                                                                ComputeSizes.Small,
                                                                                DatabaseProviders.Postgres,
                                                                                minitwitVPC.Id,
                                                                                nodecount: 1);
    minitwitDbCluster.CreateDatabase("minitwit");



    var minitwitProject = new Project("Minitwit", new ProjectArgs()
    {
        Name = "Minitwit",
        Description = "A mini Twitter clone application for the course DevOps at the IT University of Copenhagen",
        Purpose = "Application Environment",
        Resources = new InputList<string>
        {
            webServers.Select(x => x.Name).ToArray(),
            monitoringServer.Name,
            minitwitDbCluster.Name,
        }
    });


    // Export outputs here
    return new Dictionary<string, object?>
    {
        ["web"] = webServers.Select(x => x.NetworkInterfaces["PublicIP"]).ToList(),
        ["monitoring"] = new List<Output<string>>() { monitoringServer.NetworkInterfaces["PublicIP"] },
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
    // SshKey malinSSHKey = new("Malin-ssh-key", new SshKeyArgs
    // {
    //     Name = "Malin-ssh-key",
    //     PublicKey = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABgQCqHS1gbWOZrJYFmxGSwJ6I2u0sfuPt0hg5OsyF1xIgC42VITYdst5/qTB5P2xiZUo/x4f7vPiE17h/ibCDlboPl4f4zZSTExeQxHceoGxqND2r1VXxb+bwz2s6aGb8jT4qLle0Tboj3eTO61W2mNPrZbrcFpQs8Javkj9Wa1I++cOmt5jKAGc287mPOt5M/USMV09IFwH3MaYaswWZ5pZUharNAL+sqBRuYO8BoJH8gtMzLeVz3UfHETsRzbQ87XsstiH2JnlJ8PmYHC4e+9/hG9wj0O3buu5T+2HLI6asqCK/BNGNLZhe5lvlChPHb1XIrIAvfuiuxSjy6BbNexao/BYTWzCuimYajGPr2ZQzJrYPpS+n67DmsUmAOpNeaQKpPSiiixmNM9o5i8zx4n4iZa6vTO1NuBkai8QHhakIBurtx19UqsjW3HNjF7Jd0NaH80uHzAl4CFovezQS32TFL216fww6UankrkkoloXstkmpi9a5kLzDyesIVuXVReU= malin nordqvist@DESKTOP-90ST137"
    // });
    // SshKey sarahSSHKey = new("Sarah-ssh-key", new SshKeyArgs
    // {
    //     Name = "Sarah-ssh-key",
    //     PublicKey = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQD6XFydqlxARlR/NNhVKN6aqoq6ew5pClNPCp90X2R07q9k/BOJH1LEvh5agq1zyCWMdJl1/EISUmxWGawDmAp+BA+Sye3jHt80h1g/Z2bofUJZmCWsngk5dBQRuxff1tFHwjEgyuVcEfx558PKN/qSAIeoLGjOb08gCCOXTggP3j4E7/K4LBaJpOuYZOWxXFAXSMjDVA9seRKAOFlO/qw5hLYCJGlALGGbmhXPYyH5qDEdp3dX8iQEJVrF/WV+gcnl8WJHGw0bERTLWr4OERmfZP+xL5XjjTzUILQvAKx7KMNFbJ9VdjdjLUS14+uJmLfz538NUuHl2bgrVC68FOHF6G9HW9SgRW55VL59h/43YbvlHGIHYZXcvR1DZDkhOm7gN60bQpMP5w2mpygP8EdYD7ydp9gzQRM5F1CDBNMO4GpU+LT3ycUZDqlZyZsA5j/bMUwskgBGvGuHescbAlw9hYHwZds698vSxpoonrGcgXceeO2DECSgViBeXOJyTMAtNMhXhK1vrxz0jwmAqnXty28jmdxTBdceBSVkFvit4dynqyAG6IKtXWQPg0eU5PWiRplez8mlSF3ByoXjXeGAR/5YZRVRhN97Z94/v8T2/7+BNqCIX2/l1MrU9XqH0KvR5vIG+aG7NiADWSankXHMbbDiPpKbEL7BkFqvOxIDDw== sarah@Sarah-Legion"
    // });
    // SshKey milleLaptopSSHKey = new("Mille-laptop-ssh-key", new SshKeyArgs
    // {
    //     Name = "Mille-laptop-ssh-key",
    //     PublicKey = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQDFV6i58sKnXbhLSV2yIGRktPzz3UOQXPQvTZ0nA+VnieJNAPbThlo1LeNnrYjcL0dpzVZ1O4dYDiDQiwxYxq0oH/RHbVVkO7hbHhUyGWQGEm3LQzgwDxgA+/JjKpyACB9woJc7CcKYowZPajJOs3ggGw0T/QH740HaMn/QVoJhX82o7RrIYdj8PN2XqF3/tIMPSIdn4wvH0LavDBtVrZdKxG1Oog9ZJIm3SG2j9Jp2iMElgB535BlFVilAC+vPCUzCseb0Uz+ATb62P0Wjkaon/WenMH7/7Y+P/JzWJdnVJAD+DUj6t7veTIMOIIrHCiVvvnIJlcD0gc+amaSdlmxJH0sjrw1ZHzDHG880l0XlMzS1L1iyEbB5TgJJpp6sayz/84osSk+HmG8Z4jt1T/S0CCzdBQ2OTNCKwyJlQndgTiv6SXtdQlCf87JelzaFwrH01BG0Z8m/1P8FEjy3Fjio8rMd9Lq9D2X7NqANsOMIb2DE1biUgCh8DhcL8l4nnjUrO1B8KISPE7lzAuhIGkvp4jLLAuQClfCF1zF1K0wlGC5Qj3AjQKlW55nOgtU0sQ+tQmpHVMp4bPFIRxz1wHYqCNFKRTfeuASURDu4YlYw3w1LEkeyvsJ9Bf4YXt3R1HwG40nwCGnAPmkMgdh1+sJE7sMD/ZlK6mQoll3wiUSP+w== pinkvinus@ScuffedKeyboardComputerNix"
    // });
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
        // malinSSHKey.Id,
        // sarahSSHKey.Id,
        // milleLaptopSSHKey.Id,
        // millePcSSHKey.Id,
        // mySSHKey.Id,
    };
}
