using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dloizides.ArchitectureTests;

/// <summary>C2 "A repository never takes another repository; combining is a service's job".</summary>
public static class RepositoryDependencies
{
    /// <summary>Names every repository with a public constructor that takes another repository.</summary>
    public static IReadOnlyList<string> CombiningIn(IEnumerable<Assembly> assemblies) =>
        ServiceTypes.Load(assemblies)
            .Where(type => type.IsClass && !type.IsAbstract && RepositoryNames.IsRepository(type))
            .Where(TakesARepository)
            .Select(type => type.FullName ?? type.Name)
            .Distinct()
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

    private static bool TakesARepository(Type repository) =>
        repository.GetConstructors()
            .SelectMany(constructor => constructor.GetParameters())
            .Any(parameter => RepositoryNames.IsRepository(parameter.ParameterType) && !Decorates(repository, parameter.ParameterType));

    private static bool Decorates(Type repository, Type parameterType) =>
        parameterType.IsInterface && repository.GetInterfaces().Where(RepositoryNames.HasRepositoryName).SequenceEqual([parameterType]);
}
