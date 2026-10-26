using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dloizides.ArchitectureTests;

/// <summary>C2 "A repository never takes another repository; combining is a service's job".</summary>
public static class RepositoryDependencies
{
    private const string RepositorySuffix = "Repository";

    /// <summary>Names every repository with a public constructor that takes another repository.</summary>
    public static IReadOnlyList<string> CombiningIn(IEnumerable<Assembly> assemblies) =>
        ServiceTypes.Load(assemblies)
            .Where(type => type.IsClass && !type.IsAbstract && IsRepository(type))
            .Where(TakesARepository)
            .Select(type => type.FullName ?? type.Name)
            .Distinct()
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

    private static bool TakesARepository(Type repository) =>
        repository.GetConstructors()
            .SelectMany(constructor => constructor.GetParameters())
            .Any(parameter => IsRepository(parameter.ParameterType));

    private static bool IsRepository(Type type) => HasRepositoryName(type) || type.GetInterfaces().Any(HasRepositoryName);

    private static bool HasRepositoryName(Type type)
    {
        var arity = type.Name.IndexOf('`', StringComparison.Ordinal);
        var name = arity < 0 ? type.Name : type.Name[..arity];
        return name.EndsWith(RepositorySuffix, StringComparison.Ordinal);
    }
}
