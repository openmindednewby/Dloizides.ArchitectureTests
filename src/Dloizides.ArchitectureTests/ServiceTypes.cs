using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dloizides.ArchitectureTests;

internal static class ServiceTypes
{
    private const string IdSuffix = "Id";

    public static Type[] Load(IEnumerable<Assembly> assemblies) => assemblies.Distinct().SelectMany(LoadableTypes).ToArray();

    public static bool HasBodyProperty(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(property => !property.Name.EndsWith(IdSuffix, StringComparison.Ordinal));

    private static IEnumerable<Type> LoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException partial)
        {
            return partial.Types.OfType<Type>();
        }
    }
}
