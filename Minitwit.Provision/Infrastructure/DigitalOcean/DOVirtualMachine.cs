using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Pulumi;
using Pulumi.DigitalOcean;

namespace Minitwit.Provision.Infrastructure.DigitalOcean;

/// <summary>
/// Represents a virtual machine in digitalocean. This is also reffered to as a Droplet in DigitalOcean terms.
/// </summary>
internal record DOVirtualMachine : IVirtualMachine
{
    Output<string> IVirtualMachine.Name => _virtualMachine.DropletUrn;

    Output<string> IVirtualMachine.OS => _virtualMachine.Image;

    Output<string> IVirtualMachine.Region => _virtualMachine.Region;

    IDictionary<string, Output<string>> IVirtualMachine.NetworkInterfaces => _networkInterfaces;


    private readonly Droplet _virtualMachine;

    private readonly IDictionary<string, Output<string>> _networkInterfaces;

    /// <summary>
    /// Creates a new instance of a Digital Ocean virtual machine
    /// </summary>
    /// <param name="name">Name of the virtual machine</param>
    /// <param name="os">The operating system and version.
    /// You can find the list of available operating system images here https://docs.digitalocean.com/products/droplets/details/images/
    /// </param>
    /// <param name="region">The region where the virtual machine should be placed.
    /// The provided name should be the slug name.
    /// You can find the list of available regions here https://docs.digitalocean.com/products/platform/availability-matrix/#app-platform-availability
    /// </param>
    private DOVirtualMachine(string name, string os, Output<string> networkId, string region)
    {
        _virtualMachine = new Droplet(name, new()
        {
            Name = name,
            Image = os,
            Region = region,
            Size = "s-1vcpu-1gb",
            VpcUuid = networkId
        });

        _networkInterfaces = new Dictionary<string, Output<string>>
        {
            {"PublicIP",  _virtualMachine.Ipv4Address},
            {"PrivateIP", _virtualMachine.Ipv4AddressPrivate},
            {"PublicIPv6", _virtualMachine.Ipv6Address},
        };
    }

    /// <summary>
    /// Creates a new instance of a Digital Ocean virtual machine
    /// </summary>
    /// <param name="name">Name of the virtual machine</param>
    /// <param name="os">The operating system and version.
    /// You can find the list of available operating system images here https://docs.digitalocean.com/products/droplets/details/images/
    /// </param>
    /// <param name="region">The region where the virtual machine should be placed.
    /// The provided name should be the slug name.
    /// You can find the list of available regions here https://docs.digitalocean.com/products/platform/availability-matrix/#app-platform-availability
    /// </param>
    /// <param name="sshKeys">List of public ssh keys that should be able to be authorized</param>
    private DOVirtualMachine(string name, string os, Output<string> networkId, string region, InputList<string> sshKeys)
    {
        _virtualMachine = new Droplet(name, new()
        {
            Image = os,
            Name = name,
            Region = region,
            Size = "s-1vcpu-1gb",
            SshKeys = sshKeys,
            VpcUuid = networkId
        });

        _networkInterfaces = new Dictionary<string, Output<string>>
        {
            {"PublicIP",  _virtualMachine.Ipv4Address},
            {"PrivateIP", _virtualMachine.Ipv4AddressPrivate},
            {"PublicIPv6", _virtualMachine.Ipv6Address},
        };
    }

    /// <summary>
    /// Creates a Virtual Machine a.k.a. droplet
    /// </summary>
    /// <param name="name">Name of the virtual machine</param>
    /// <param name="os">The operating system and version. You can find the list of available operating system images here https://docs.digitalocean.com/products/droplets/details/images/</param>
    /// <param name="region">The region where the virtual machine should be placed</param>
    /// <returns>Returns the virtual machine</returns>
    public static IVirtualMachine CreateVM(string name, string os, Output<string> networkId, string region)
        => new DOVirtualMachine(name, os, networkId, region);

    /// <summary>
    /// Creates a Virtual Machine a.k.a. droplet
    /// </summary>
    /// <param name="name">Name of the virtual machine</param>
    /// <param name="os">The operating system and version. You can find the list of available operating system images here https://docs.digitalocean.com/products/droplets/details/images/</param>
    /// <param name="region">The region where the virtual machine should be placed</param>
    /// <param name="sshKeys">SSH keys to be authorized on the server</param>
    /// <returns>Returns the virtual machine</returns>
    public static IVirtualMachine CreateVM(string name, string os, Output<string> networkId, string region, InputList<string> sshKeys)
        => new DOVirtualMachine(name, os, networkId, region, sshKeys);

    /// <summary>
    /// Creates a set of similar virtual machines
    /// </summary>
    /// <param name="setNamePrefix">The prefix name of the set of virtual machines</param>
    /// <param name="os">The operating system and version. You can find the list of available operating system images here https://docs.digitalocean.com/products/droplets/details/images/</param>
    /// <param name="region">The region where the virtual machine should be placed</param>
    /// <param name="count">The number of virtual machines to be created</param>
    /// <returns>Returns an enumerable of virtual machines</returns>
    public static IEnumerable<IVirtualMachine> CreateVMSet(string setNamePrefix, string os, Output<string> networkId, string region, int count = 1, InputList<string>? sshKeys = null)
    {
        string setName = setNamePrefix[^1] == '-' ? setNamePrefix : $"{setNamePrefix}-"; // Create the prefix seperator

        // Creates a collection of VM's with the following naming scheme: <VM Set Name>-<VM Number>
        return sshKeys is null ? Enumerable.Range(1, count).Select(vmNumber => CreateVM($"{setName}{vmNumber}", os, networkId, region)).ToList()
                                : Enumerable.Range(1, count).Select(vmNumber => CreateVM($"{setName}{vmNumber}", os, networkId, region, sshKeys)).ToList();
    }
}
