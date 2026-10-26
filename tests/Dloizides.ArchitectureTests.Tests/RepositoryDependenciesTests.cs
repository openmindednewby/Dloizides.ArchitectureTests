using Clean.Web;
using Mvc.Service;
using Xunit;

namespace Dloizides.ArchitectureTests.Tests;

public sealed class RepositoryDependenciesTests
{
    [Fact]
    public void CombiningIn_WithRepositoriesTakingRepositories_NamesOnlyThoseRepositories()
    {
        var assemblies = new[] { typeof(OrderRepository).Assembly };
        var expected = new[] { typeof(LedgerStore).FullName!, typeof(OrderRepository).FullName! };

        var combining = RepositoryDependencies.CombiningIn(assemblies);

        Assert.Equal(expected, combining);
    }

    [Fact]
    public void CombiningIn_WithoutRepositories_ReportsNothing()
    {
        var assemblies = new[] { typeof(CleanWebMarker).Assembly };

        var combining = RepositoryDependencies.CombiningIn(assemblies);

        Assert.Empty(combining);
    }
}
