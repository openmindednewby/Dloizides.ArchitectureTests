using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dloizides.ArchitectureTests;

/// <summary>PAT-REVIEW-15 "Validation at the edge with a validator per request" for MVC controllers.</summary>
public static class ControllerRequestValidators
{
    private const string ControllerBase = "Microsoft.AspNetCore.Mvc.ControllerBase";
    private const string NonAction = "Microsoft.AspNetCore.Mvc.NonActionAttribute";
    private const string FluentValidator = "FluentValidation.IValidator`1";

    private static readonly HashSet<string> NonRequestSources = new(StringComparer.Ordinal)
    {
        "Microsoft.AspNetCore.Mvc.FromRouteAttribute",
        "Microsoft.AspNetCore.Mvc.FromHeaderAttribute",
        "Microsoft.AspNetCore.Mvc.FromServicesAttribute",
        "Microsoft.AspNetCore.Mvc.FromKeyedServicesAttribute",
    };

    /// <summary>Names every action request with a non-id property and no FluentValidation validator.</summary>
    public static IReadOnlyList<string> MissingIn(IEnumerable<Assembly> assemblies)
    {
        var types = ServiceTypes.Load(assemblies);
        var own = types.ToHashSet();
        var validated = types.Where(IsConcreteClass).SelectMany(ValidatedTypes).ToHashSet();
        return types
            .Where(IsController)
            .SelectMany(ActionRequests)
            .Distinct()
            .Where(type => own.Contains(type) && ServiceTypes.HasBodyProperty(type) && !validated.Contains(type))
            .Select(type => type.FullName ?? type.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
    }

    private static bool IsConcreteClass(Type type) => type.IsClass && !type.IsAbstract && !type.ContainsGenericParameters;

    private static bool IsController(Type type)
    {
        if (!IsConcreteClass(type))
            return false;
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (string.Equals(current.FullName, ControllerBase, StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    private static IEnumerable<Type> ActionRequests(Type controller) =>
        controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(method => !method.IsSpecialName && !HasAttribute(method.CustomAttributes, NonAction))
            .SelectMany(method => method.GetParameters())
            .Where(parameter => !parameter.CustomAttributes.Any(attribute => NonRequestSources.Contains(attribute.AttributeType.FullName ?? string.Empty)))
            .Select(parameter => parameter.ParameterType);

    private static bool HasAttribute(IEnumerable<CustomAttributeData> attributes, string fullName) =>
        attributes.Any(attribute => string.Equals(attribute.AttributeType.FullName, fullName, StringComparison.Ordinal));

    private static IEnumerable<Type> ValidatedTypes(Type type) =>
        type.GetInterfaces()
            .Where(contract => contract.IsGenericType && string.Equals(contract.GetGenericTypeDefinition().FullName, FluentValidator, StringComparison.Ordinal))
            .Select(contract => contract.GetGenericArguments()[0]);
}
