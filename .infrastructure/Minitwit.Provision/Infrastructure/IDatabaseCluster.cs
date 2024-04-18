using System.Collections.Generic;
using Pulumi;

namespace Minitwit.Provision;

/// <summary>
/// Represents a Database
/// </summary>
public interface IDatabaseCluster
{
    /// <summary>
    /// Name of the database
    /// </summary>
    Output<string> Name { get; }

    /// <summary>
    /// Amount of node instances in the cluster
    /// </summary>
    Output<int> NodeCount { get; }

    /// <summary>
    /// The Database provider name or product
    /// </summary>
    Output<string> Provider { get; }

    /// <summary>
    /// A list of databases that the cluster contains
    /// </summary>
    Output<IEnumerable<string>> Databases { get; }

    /// <summary>
    /// Creates a new database in the database cluster with a given name
    /// </summary>
    /// <param name="name">Name of the database</param>
    void CreateDatabase(string name);
}
