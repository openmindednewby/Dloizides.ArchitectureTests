using Clean.Web;
using Mvc.Service;
using Xunit;

namespace Dloizides.ArchitectureTests.Tests;

public sealed class ControllerRoutesTests
{
    [Fact]
    public void SharedIn_WithControllersOnOneTemplate_NamesEveryControllerSharingIt()
    {
        var assemblies = new[] { typeof(CasesController).Assembly };
        var expected = new[]
        {
            typeof(CaseArchiveController).FullName!,
            typeof(CasesController).FullName!,
            typeof(DailyReportsController).FullName!,
            typeof(IngestController).FullName!,
            typeof(IngestJobController).FullName!,
            typeof(MonthlyReportsController).FullName!,
        };

        var shared = ControllerRoutes.SharedIn(assemblies);

        Assert.Equal(expected, shared);
    }

    [Fact]
    public void SharedIn_WithRouteInheritedFromBaseController_NamesBothControllers()
    {
        var assemblies = new[] { typeof(DailyReportsController).Assembly };

        var shared = ControllerRoutes.SharedIn(assemblies);

        Assert.Contains(typeof(DailyReportsController).FullName!, shared);
        Assert.Contains(typeof(MonthlyReportsController).FullName!, shared);
    }

    [Fact]
    public void SharedIn_WithFastEndpointsServiceWithoutControllers_ReportsNothing()
    {
        var assemblies = new[] { typeof(CleanWebMarker).Assembly };

        var shared = ControllerRoutes.SharedIn(assemblies);

        Assert.Empty(shared);
    }
}
