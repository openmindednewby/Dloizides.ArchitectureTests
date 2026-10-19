using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Dloizides.ArchitectureTests;

internal static class LayerAssemblies
{
    private static readonly IReadOnlyList<KeyValuePair<string, ServiceLayer>> SuffixLayers =
    [
        new(".Domain", ServiceLayer.Domain),
        new(".Core", ServiceLayer.Domain),
        new(".Application", ServiceLayer.Application),
        new(".UseCases", ServiceLayer.Application),
        new(".Infrastructure", ServiceLayer.Infrastructure),
        new(".Web", ServiceLayer.Web),
        new(".API", ServiceLayer.Web),
        new(".Api", ServiceLayer.Web),
    ];

    public static IReadOnlyDictionary<ServiceLayer, Assembly[]> Find(Assembly webAssembly)
    {
        var webName = webAssembly.GetName().Name ?? string.Empty;
        var webSuffix = SuffixLayers
            .Where(pair => pair.Value == ServiceLayer.Web && webName.EndsWith(pair.Key, StringComparison.Ordinal))
            .Select(pair => pair.Key)
            .FirstOrDefault()
            ?? throw new ArgumentException($"{webName} does not end with .Web, .API or .Api, so it is not a web assembly.", nameof(webAssembly));
        var prefix = webName[..^webSuffix.Length];
        var directory = Path.GetDirectoryName(webAssembly.Location) ?? AppContext.BaseDirectory;

        return SuffixLayers
            .Select(pair => (Layer: pair.Value, Assembly: Resolve(prefix + pair.Key, webAssembly, directory)))
            .Where(found => found.Assembly is not null)
            .GroupBy(found => found.Layer, found => found.Assembly!)
            .ToDictionary(group => group.Key, group => group.Distinct().ToArray());
    }

    private static Assembly? Resolve(string name, Assembly webAssembly, string directory)
    {
        if (string.Equals(webAssembly.GetName().Name, name, StringComparison.Ordinal))
            return webAssembly;
        return File.Exists(Path.Combine(directory, name + ".dll")) ? Assembly.Load(new AssemblyName(name)) : null;
    }
}
