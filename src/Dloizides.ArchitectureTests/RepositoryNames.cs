using System;
using System.Linq;

namespace Dloizides.ArchitectureTests;

internal static class RepositoryNames
{
    private const string RepositorySuffix = "Repository";

    public static bool IsRepository(Type type) => HasRepositoryName(type) || type.GetInterfaces().Any(HasRepositoryName);

    public static bool HasRepositoryName(Type type)
    {
        var arity = type.Name.IndexOf('`', StringComparison.Ordinal);
        var name = arity < 0 ? type.Name : type.Name[..arity];
        return name.EndsWith(RepositorySuffix, StringComparison.Ordinal);
    }
}
