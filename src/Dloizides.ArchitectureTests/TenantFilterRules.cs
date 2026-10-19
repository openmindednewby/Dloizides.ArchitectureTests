using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Dloizides.ArchitectureTests;

/// <summary>The tenant query-filter rule; derive one test class per DbContext that holds tenant data.</summary>
public abstract class TenantFilterRules
{
    [Fact]
    public void TenantEntities_WhenModelChecked_EachHasAQueryFilter() => Assert.Empty(TenantQueryFilters.MissingIn(CreateModel()));

    /// <summary>Returns the model of the DbContext under test, for example context.Model.</summary>
    protected abstract IReadOnlyModel CreateModel();
}
