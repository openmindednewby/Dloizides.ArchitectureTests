using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Dloizides.ArchitectureTests;

/// <summary>The tenant query-filter rule for the service next to TWebMarker; one test class per DbContext.</summary>
public abstract class TenantFilterRules<TWebMarker>
{
    /// <summary>Returns the tenant entities read across tenants on purpose, each with its reason.</summary>
    protected virtual IEnumerable<ArchitectureExemption> Exemptions => [];

    [Fact]
    public void TenantEntities_WhenModelChecked_EachHasAQueryFilter() => Assert.Empty(ExemptionFilter.Unexempted(Missing(), Exemptions));

    [Fact]
    public void TenantFilterExemptions_WhenModelChecked_EachStillLacksAQueryFilter() => Assert.Empty(ExemptionFilter.Stale(Missing(), Exemptions));

    /// <summary>Returns the model of the DbContext under test, for example context.Model.</summary>
    protected abstract IReadOnlyModel CreateModel();

    private IReadOnlyList<string> Missing() => TenantQueryFilters.MissingIn(CreateModel(), LayerAssemblies.Find(typeof(TWebMarker).Assembly).Values.SelectMany(layer => layer));
}
