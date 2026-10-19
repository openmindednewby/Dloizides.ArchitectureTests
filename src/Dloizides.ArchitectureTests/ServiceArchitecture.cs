using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Architecture = ArchUnitNET.Domain.Architecture;
using ArchUnitNET.Loader;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Dloizides.ArchitectureTests;

/// <summary>One service's layer assemblies, loaded once, checked against PAT-LAYER-1.</summary>
public sealed class ServiceArchitecture
{
    private readonly Architecture _architecture;

    private ServiceArchitecture(IReadOnlyDictionary<ServiceLayer, Assembly[]> layers, Architecture architecture)
    {
        Layers = layers;
        _architecture = architecture;
    }

    /// <summary>The layers found, each with its assemblies; a missing layer has no entry.</summary>
    public IReadOnlyDictionary<ServiceLayer, Assembly[]> Layers { get; }

    /// <summary>Loads the layers that sit next to the given web assembly.</summary>
    public static ServiceArchitecture Load(Assembly webAssembly)
    {
        var layers = LayerAssemblies.Find(webAssembly);
        var architecture = new ArchLoader().LoadAssemblies(layers.Values.SelectMany(assemblies => assemblies).ToArray()).Build();
        return new ServiceArchitecture(layers, architecture);
    }

    /// <summary>Describes every type in the layer that uses a type of an outer layer.</summary>
    public IReadOnlyList<string> ViolationsOf(ServiceLayer layer)
    {
        if (!Layers.TryGetValue(layer, out var inner))
            return [];
        var outer = Layers.Where(pair => pair.Key > layer).SelectMany(pair => pair.Value).ToArray();
        if (outer.Length == 0)
            return [];

        var rule = Types().That().ResideInAssembly(inner[0], inner[1..])
            .Should().NotDependOnAny(Types().That().ResideInAssembly(outer[0], outer[1..]))
            .Because($"PAT-LAYER-1 \"Clean Architecture layer direction\": {layer} must not use an outer layer");
        return rule.Evaluate(_architecture).Where(result => !result.Passed).Select(result => result.Description).ToArray();
    }
}
