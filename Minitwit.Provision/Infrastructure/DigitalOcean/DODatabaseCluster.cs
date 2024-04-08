using System.Collections.Generic;
using Pulumi;
using Pulumi.DigitalOcean;
using static Minitwit.Provision.Infrastructure.DatabaseProviders;
using static Minitwit.Provision.Infrastructure.ComputeSizes;
using System;

namespace Minitwit.Provision.Infrastructure.DigitalOcean;

/// <summary>
/// Represents a database cluster in Digital Ocean. This is what you would call a managed service.
/// </summary>
internal record DODatabaseCluster : IDatabaseCluster
{
    public Output<string> Name => _cluster.ClusterUrn;

    public Output<int> NodeCount => _cluster.NodeCount;

    public Output<string> Provider => _cluster.Engine;

    public Output<IEnumerable<string>> Databases => throw new System.NotImplementedException();


    private readonly DatabaseCluster _cluster;

    /// <summary>
    /// Creates DigitalOcean managed Database cluster
    /// </summary>
    /// <param name="name">Name of the cluster</param>
    /// <param name="version">Version of the cluster</param>
    /// <param name="size">Size of the compute nodes</param>
    /// <param name="provider">Which type of the database to create</param>
    /// <param name="nodecount">Amount of nodes that cluster should have</param>
    /// <param name="region">Where it is located</param>
    private DODatabaseCluster(string name, string version, string size, string provider, Output<string> networkId, int nodecount, string region)
    {
        _cluster = new DatabaseCluster(name, new()
        {
            Engine = provider,
            Version = version,
            Size = size,
            Region = region,
            NodeCount = nodecount,
            PrivateNetworkUuid = networkId
        });
    }

    /// <summary>
    /// Creates a Database cluster with the specific name
    /// </summary>
    /// <param name="name">Name of the database cluster</param>
    /// <param name="size">Compute size of the cluster</param>
    /// <param name="provider">Which type of database</param>
    /// <param name="nodecount">How many nodes the cluster should have</param>
    /// <returns>A database cluster</returns>
    public static IDatabaseCluster CreateDatabaseCluster(string name, ComputeSizes size, DatabaseProviders provider, Output<string> networkId, int nodecount)
        => new DODatabaseCluster(name, GetProviderVersion(provider), GetComputeSize(size), GetProviderName(provider), networkId, nodecount, "fra1");

    /// <summary>
    /// Gets the provider key for DigitalOcean
    /// </summary>
    /// <param name="provider">Provider type to use in digital ocean</param>
    /// <returns>A DigitalOcean specific key</returns>
    private static string GetProviderName(DatabaseProviders provider) => provider switch
    {
        Postgres => "pg",
        MySQL => "mysql",
        MongoDb => "mongodb",
        _ => throw new NotSupportedException($"The provider {provider} is not supported in DigitalOcean")
    };

    private static string GetProviderVersion(DatabaseProviders provider) => provider switch
    {
        Postgres => "16",
        MySQL => "8",
        MongoDb => "6",
        _ => throw new NotSupportedException($"The provider {provider} is not supported in DigitalOcean")
    };

    private static string GetComputeSize(ComputeSizes size) => size switch
    {
        Small => "db-s-1vcpu-1gb",
        Medium => "db-s-2vcpu-4gb",
        Large => "db-s-4vcpu-8gb",
        _ => throw new NotSupportedException($"The size {size} is not supported for Database Nodes in DigitalOcean Databases")
    };

    public void CreateDatabase(string name)
    {
        new DatabaseDb(name, new()
        {
            Name = name,
            ClusterId = _cluster.Id, // TODO: Add db name in the new args
        });
    }
}
