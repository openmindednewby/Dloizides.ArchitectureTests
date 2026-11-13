using Clean.Web;
using Mvc.Service;
using Xunit;

namespace Dloizides.ArchitectureTests.Tests;

public sealed class ControllerDependenciesTests
{
    [Fact]
    public void TakingRepositoryIn_WithControllersAndEndpointsTakingRepositories_NamesOnlyThose()
    {
        var assemblies = new[] { typeof(CustomersController).Assembly };
        var expected = new[] { typeof(CustomersController).FullName!, typeof(OrderExportEndpoint).FullName!, typeof(ProductListEndpoint).FullName! };

        var offenders = ControllerDependencies.TakingRepositoryIn(assemblies);

        Assert.Equal(expected, offenders);
    }

    [Fact]
    public void TakingRepositoryIn_WithServiceTakingRepository_DoesNotNameIt()
    {
        var assemblies = new[] { typeof(LedgerReportService).Assembly };

        var offenders = ControllerDependencies.TakingRepositoryIn(assemblies);

        Assert.DoesNotContain(typeof(LedgerReportService).FullName!, offenders);
    }

    [Fact]
    public void TakingRepositoryIn_WithControllerTakingOnlyAService_DoesNotNameIt()
    {
        var assemblies = new[] { typeof(BillingController).Assembly };

        var offenders = ControllerDependencies.TakingRepositoryIn(assemblies);

        Assert.DoesNotContain(typeof(BillingController).FullName!, offenders);
    }

    [Fact]
    public void TakingRepositoryIn_WithoutControllersOrEndpoints_ReportsNothing()
    {
        var assemblies = new[] { typeof(CleanWebMarker).Assembly };

        var offenders = ControllerDependencies.TakingRepositoryIn(assemblies);

        Assert.Empty(offenders);
    }
}
