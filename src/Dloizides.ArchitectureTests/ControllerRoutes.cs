using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dloizides.ArchitectureTests;

/// <summary>C3 "One controller per route prefix; new actions on a resource join its controller".</summary>
public static class ControllerRoutes
{
    private const string RouteAttribute = "Microsoft.AspNetCore.Mvc.RouteAttribute";
    private const string ControllerSegment = "[controller]";
    private const string ControllerSuffix = "Controller";

    /// <summary>Names every MVC controller whose class-level route template another controller also declares.</summary>
    public static IReadOnlyList<string> SharedIn(IEnumerable<Assembly> assemblies) =>
        ServiceTypes.Load(assemblies)
            .Where(ControllerRequestValidators.IsController)
            .SelectMany(controller => Templates(controller).Select(template => (Controller: controller, Template: template)))
            .GroupBy(route => route.Template, StringComparer.Ordinal)
            .Where(group => group.Select(route => route.Controller).Distinct().Count() > 1)
            .SelectMany(group => group.Select(route => route.Controller.FullName ?? route.Controller.Name))
            .Distinct()
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

    private static IEnumerable<string> Templates(Type controller) =>
        controller.CustomAttributes
            .Where(attribute => string.Equals(attribute.AttributeType.FullName, RouteAttribute, StringComparison.Ordinal))
            .Select(attribute => attribute.ConstructorArguments.Count > 0 ? attribute.ConstructorArguments[0].Value as string : null)
            .OfType<string>()
            .Select(template => Normalize(template, controller.Name));

    private static string Normalize(string template, string controllerName)
    {
        var resource = controllerName.EndsWith(ControllerSuffix, StringComparison.Ordinal) ? controllerName[..^ControllerSuffix.Length] : controllerName;
        return template.Replace(ControllerSegment, resource, StringComparison.OrdinalIgnoreCase).Trim('/').ToUpperInvariant();
    }
}
