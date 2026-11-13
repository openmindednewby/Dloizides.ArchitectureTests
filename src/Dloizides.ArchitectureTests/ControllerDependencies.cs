using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dloizides.ArchitectureTests;

/// <summary>CONTROLLER-SERVICE-1 "A controller calls a service, never a repository".</summary>
public static class ControllerDependencies
{
    private const string EndpointBasePrefix = "Endpoint`";
    private const string EndpointWithoutRequest = "EndpointWithoutRequest";

    /// <summary>Names every controller or endpoint whose public constructor takes a repository.</summary>
    public static IReadOnlyList<string> TakingRepositoryIn(IEnumerable<Assembly> assemblies) =>
        ServiceTypes.Load(assemblies)
            .Where(type => type.IsClass && !type.IsAbstract && (ControllerRequestValidators.IsController(type) || IsEndpoint(type)))
            .Where(TakesARepository)
            .Select(type => type.FullName ?? type.Name)
            .Distinct()
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

    private static bool TakesARepository(Type caller) =>
        caller.GetConstructors()
            .SelectMany(constructor => constructor.GetParameters())
            .Any(parameter => RepositoryNames.IsRepository(parameter.ParameterType));

    private static bool IsEndpoint(Type type)
    {
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            var isGenericEndpoint = current.IsGenericType && current.GetGenericTypeDefinition().Name.StartsWith(EndpointBasePrefix, StringComparison.Ordinal);
            if (isGenericEndpoint || string.Equals(current.Name, EndpointWithoutRequest, StringComparison.Ordinal))
                return true;
        }

        return false;
    }
}
