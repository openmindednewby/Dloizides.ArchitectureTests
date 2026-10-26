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
        var expected = new[] { typeof(CachedPriceRepository).FullName!, typeof(CatalogRepository).FullName!, typeof(LedgerStore).FullName!, typeof(OrderRepository).FullName! };

        var combining = RepositoryDependencies.CombiningIn(assemblies);

        Assert.Equal(expected, combining);
    }

    [Fact]
    public void CombiningIn_WithDecoratorTakingTheRepositoryItImplements_DoesNotNameIt()
    {
        var assemblies = new[] { typeof(CachedProductRepository).Assembly };

        var combining = RepositoryDependencies.CombiningIn(assemblies);

        Assert.DoesNotContain(typeof(CachedProductRepository).FullName!, combining);
    }

    [Fact]
    public void CombiningIn_WithDecoratorAlsoTakingAnotherRepository_NamesIt()
    {
        var assemblies = new[] { typeof(CachedPriceRepository).Assembly };

        var combining = RepositoryDependencies.CombiningIn(assemblies);

        Assert.Contains(typeof(CachedPriceRepository).FullName!, combining);
    }

    [Fact]
    public void CombiningIn_WithRepositoryImplementingTwoRepositoryInterfaces_NamesIt()
    {
        var assemblies = new[] { typeof(CatalogRepository).Assembly };

        var combining = RepositoryDependencies.CombiningIn(assemblies);

        Assert.Contains(typeof(CatalogRepository).FullName!, combining);
    }

    [Fact]
    public void CombiningIn_WithoutRepositories_ReportsNothing()
    {
        var assemblies = new[] { typeof(CleanWebMarker).Assembly };

        var combining = RepositoryDependencies.CombiningIn(assemblies);

        Assert.Empty(combining);
    }
}
