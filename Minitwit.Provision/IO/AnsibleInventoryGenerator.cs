using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minitwit.Provision.IO;

public static class AnsibleInventoryGenerator
{
    public static string Generate(IDictionary<string, IEnumerable<string>> servers)
    {
        StringBuilder sb = new StringBuilder();

        // Add all servers category to the inventory content
        var allServers = servers.SelectMany(x => x.Value);
        sb.AppendLine("[all]");
        foreach (var ip in allServers)
            sb.AppendLine(ip);

        sb.AppendLine(); // Provide some space between categories

        // Go through each server category and create ansible inventory sectino
        foreach (var serverCategory in servers)
        {
            sb.AppendLine($"[{serverCategory.Key}]");
            foreach (var ip in serverCategory.Value)
                sb.AppendLine(ip);

            sb.AppendLine(); // Provide some space between categories
        }

        return sb.ToString();
    }
}
