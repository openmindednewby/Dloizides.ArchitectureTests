using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Xunit;

namespace Dloizides.ArchitectureTests;

/// <summary>Finds the registered re-implementations of shared packages in a service assembly.</summary>
public static class ReimplementedPackageRules
{
    /// <summary>Returns every type of the assembly that re-implements a registered package.</summary>
    public static IReadOnlyList<ReimplementedPackageFinding> MissingIn(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        using var module = ModuleDefinition.ReadModule(assembly.Location);
        return module.GetTypes()
            .SelectMany(type => ReimplementedPackages.All
                .Where(package => Declares(type, package) || References(type, package))
                .Select(package => new ReimplementedPackageFinding(TopLevelName(type), package)))
            .Distinct()
            .OrderBy(finding => finding.TypeName, StringComparer.Ordinal)
            .ThenBy(finding => finding.Package.Pattern, StringComparer.Ordinal)
            .ToArray();
    }

    /// <summary>Fails when a finding is not exempted or an exemption no longer matches a finding.</summary>
    public static void AssertNone(Assembly assembly, params ArchitectureExemption[] exemptions)
    {
        var findings = MissingIn(assembly);
        var names = findings.Select(finding => finding.TypeName).Distinct().ToArray();
        var unexempted = ExemptionFilter.Unexempted(names, exemptions).ToHashSet(StringComparer.Ordinal);
        var failures = findings
            .Where(finding => unexempted.Contains(finding.TypeName))
            .Select(finding => finding.ToString())
            .Concat(ExemptionFilter.Stale(names, exemptions).Select(name => $"{name} is exempted but re-implements no package any more; remove the exemption"))
            .ToArray();
        if (failures.Length > 0)
            Assert.Fail(string.Join(Environment.NewLine, failures));
    }

    private static bool Declares(TypeDefinition type, ReimplementedPackage package) => package.DeclaredTypeNames.Contains(type.Name, StringComparer.Ordinal);

    private static bool References(TypeDefinition type, ReimplementedPackage package) =>
        package.ReferencedTypes.Count > 0 && !package.ReferencedTypes.Contains(TopLevelName(type), StringComparer.Ordinal) && type.Methods
            .Where(method => method.HasBody)
            .SelectMany(method => method.Body.Instructions)
            .Select(instruction => ReferencedTypeName(instruction.Operand))
            .Any(name => name is not null && package.ReferencedTypes.Contains(name, StringComparer.Ordinal));

    private static string? ReferencedTypeName(object? operand) => operand switch
    {
        TypeReference type => type.FullName,
        MemberReference { DeclaringType: { } declaring } => declaring.FullName,
        _ => null,
    };

    private static string TopLevelName(TypeDefinition type)
    {
        var current = type;
        while (current.DeclaringType is not null)
            current = current.DeclaringType;
        return current.FullName;
    }
}
