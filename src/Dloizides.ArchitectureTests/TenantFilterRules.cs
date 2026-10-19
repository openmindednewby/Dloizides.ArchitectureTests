using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Dloizides.ArchitectureTests;

/// <summary>The tenant query-filter rule for the service next to TWebMarker; one test class per DbContext.</summary>
public abstract class TenantFilterRules<TWebMarker>
{
    [Fact]
    public void TenantEntities_WhenModelChecked_EachHasAQueryFilter() => Assert.Empty(TenantQueryFilters.MissingIn(CreateModel(), LayerAssemblies.Find(typeof(TWebMarker).Assembly).Values.SelectMany(layer => layer)));

    /// <summary>Returns the model of the DbContext under test, for example context.Model.</summary>
    protected abstract IReadOnlyModel CreateModel();
}
