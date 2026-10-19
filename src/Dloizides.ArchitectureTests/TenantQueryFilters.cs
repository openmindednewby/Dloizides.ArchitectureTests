using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Dloizides.ArchitectureTests;

/// <summary>PAT-REVIEW-12 "Every tenant-scoped query filters by tenant in one place".</summary>
public static class TenantQueryFilters
{
    private const string TenantProperty = "TenantId";
    private const string QueryFilterAnnotation = "QueryFilter";

    /// <summary>Names every root entity with a TenantId property and no query filter.</summary>
    public static IReadOnlyList<string> MissingIn(IReadOnlyModel model) =>
        model.GetEntityTypes()
            .Where(entity => entity.BaseType is null && !entity.IsOwned() && entity.FindProperty(TenantProperty) is not null && !HasQueryFilter(entity))
            .Select(entity => entity.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

    private static bool HasQueryFilter(IReadOnlyEntityType entity) =>
        entity.GetQueryFilter() is not null
        || entity.GetAnnotations().Any(annotation => annotation.Name.Contains(QueryFilterAnnotation, StringComparison.Ordinal) && annotation.Value is not null);
}
