using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dloizides.ArchitectureTests;

/// <summary>PAT-REVIEW-15 "Validation at the edge with a validator per request".</summary>
public static class RequestValidators
{
    private const string EndpointBasePrefix = "Endpoint`";
    private const string IdSuffix = "Id";
    private const string FastEndpointsValidator = "FastEndpoints.Validator`1";

    /// <summary>Names every endpoint request with a non-id property and no FastEndpoints <c>Validator&lt;T&gt;</c>.</summary>
    public static IReadOnlyList<string> MissingIn(IEnumerable<Assembly> assemblies)
    {
        var types = assemblies.Distinct().SelectMany(LoadableTypes).ToArray();
        var own = types.ToHashSet();
        var validated = types.SelectMany(ValidatedTypes).ToHashSet();
        return types
            .SelectMany(EndpointRequests)
            .Distinct()
            .Where(type => own.Contains(type) && HasBodyProperty(type) && !validated.Contains(type))
            .Select(type => type.FullName ?? type.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
    }

    private static bool HasBodyProperty(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(property => !property.Name.EndsWith(IdSuffix, StringComparison.Ordinal));

    private static IEnumerable<Type> EndpointRequests(Type type)
    {
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition().Name.StartsWith(EndpointBasePrefix, StringComparison.Ordinal))
                yield return current.GetGenericArguments()[0];
        }
    }

    private static IEnumerable<Type> ValidatedTypes(Type type)
    {
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (current.IsGenericType && string.Equals(current.GetGenericTypeDefinition().FullName, FastEndpointsValidator, StringComparison.Ordinal))
                yield return current.GetGenericArguments()[0];
        }
    }

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
