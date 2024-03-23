using System.Collections.Generic;

namespace Minitwit.Provision.Infrastructure;

/// <summary>
/// Represents a virtual machine
/// </summary>
internal interface IVirtualMachine
{
    /// <summary>
    /// Name of the virtual machine
    /// </summary>
    Pulumi.Output<string> Name { get; }

    /// <summary>
    /// Operating System of the virtual machine
    /// </summary>
    Pulumi.Output<string> OS { get; }

    /// <summary>
    /// Which region the virtual machine is located
    /// </summary>
    Pulumi.Output<string> Region { get; }

    /// <summary>
    /// Network interfaces of the virtual machine. That is, a dictionary of IP addresses indicating if they are private/public or IPv4/IPv6
    /// </summary>
    IDictionary<string, Pulumi.Output<string>> NetworkInterfaces { get; } // TODO: Change to a real network interface type that keeps more information
}
