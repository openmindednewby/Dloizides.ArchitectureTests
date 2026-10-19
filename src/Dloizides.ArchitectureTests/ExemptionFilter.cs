using System;
using System.Collections.Generic;
using System.Linq;

namespace Dloizides.ArchitectureTests;

/// <summary>Splits a rule's findings by the rule's exemptions.</summary>
public static class ExemptionFilter
{
    /// <summary>Returns the findings no exemption names.</summary>
    public static IReadOnlyList<string> Unexempted(IEnumerable<string> findings, IEnumerable<ArchitectureExemption> exemptions)
    {
        var exempted = exemptions.Select(exemption => exemption.Name).ToHashSet(StringComparer.Ordinal);
        return findings.Where(finding => !exempted.Contains(finding)).ToArray();
    }

    /// <summary>Returns the exempted types that no longer break the rule.</summary>
    public static IReadOnlyList<string> Stale(IEnumerable<string> findings, IEnumerable<ArchitectureExemption> exemptions)
    {
        var found = findings.ToHashSet(StringComparer.Ordinal);
        return exemptions
            .Select(exemption => exemption.Name)
            .Where(name => !found.Contains(name))
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
    }
}
