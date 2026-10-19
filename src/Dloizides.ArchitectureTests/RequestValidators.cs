using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dloizides.ArchitectureTests;

/// <summary>PAT-REVIEW-15 "Validation at the edge with a validator per request".</summary>
public static class RequestValidators
{
    private static readonly string[] ValidatorBases = ["AbstractValidator`1", "Validator`1"];

    /// <summary>Names every public *Request type with properties that no validator in the assemblies validates.</summary>
    public static IReadOnlyList<string> MissingIn(IEnumerable<Assembly> assemblies)
    {
        var types = assemblies.Distinct().SelectMany(LoadableTypes).ToArray();
        var validated = types.SelectMany(ValidatedTypes).ToHashSet();
        return types
            .Where(type => IsRequest(type) && !validated.Contains(type))
            .Select(type => type.FullName ?? type.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
    }

    private static bool IsRequest(Type type) =>
        type.IsClass
        && type.IsVisible
        && !type.IsAbstract
        && !type.IsGenericTypeDefinition
        && type.Name.EndsWith("Request", StringComparison.Ordinal)
        && type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Length > 0;

    private static IEnumerable<Type> ValidatedTypes(Type type)
    {
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (current.IsGenericType && ValidatorBases.Contains(current.GetGenericTypeDefinition().Name, StringComparer.Ordinal))
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
