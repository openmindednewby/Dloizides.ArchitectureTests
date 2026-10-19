using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Dloizides.ArchitectureTests;

/// <summary>PAT-REVIEW-12 "Every tenant-scoped query filters by tenant in one place".</summary>
public static class TenantQueryFilters
{
    private const string TenantProperty = "TenantId";
    private const string QueryFilterAnnotation = "QueryFilter";

    /// <summary>Names every service-declared root entity with a TenantId and no query filter.</summary>
    public static IReadOnlyList<string> MissingIn(IReadOnlyModel model, IEnumerable<Assembly> serviceAssemblies)
    {
        var own = serviceAssemblies.ToHashSet();
        return model.GetEntityTypes()
            .Where(entity => own.Contains(entity.ClrType.Assembly) && entity.BaseType is null && !entity.IsOwned() && entity.FindProperty(TenantProperty) is not null && !HasQueryFilter(entity))
            .Select(entity => entity.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
    }

    private static bool HasQueryFilter(IReadOnlyEntityType entity) =>
        entity.GetQueryFilter() is not null
        || entity.GetAnnotations().Any(annotation => annotation.Name.Contains(QueryFilterAnnotation, StringComparison.Ordinal) && annotation.Value is not null);
}
