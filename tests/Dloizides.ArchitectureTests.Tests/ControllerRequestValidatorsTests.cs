using Mvc.Service;
using Xunit;

namespace Dloizides.ArchitectureTests.Tests;

public sealed class ControllerRequestValidatorsTests
{
    [Fact]
    public void MissingIn_WithUnvalidatedBodyAndQueryRequests_NamesOnlyThoseRequests()
    {
        var assemblies = new[] { typeof(CasesController).Assembly };
        var expected = new[] { typeof(CaseSearchQuery).FullName!, typeof(CloseCaseRequest).FullName! };

        var missing = ControllerRequestValidators.MissingIn(assemblies);

        Assert.Equal(expected, missing);
    }

    [Fact]
    public void MissingIn_WithControllerOutsideTheGivenAssemblies_ReportsNothing()
    {
        var assemblies = new[] { typeof(ControllerRequestValidatorsTests).Assembly };

        var missing = ControllerRequestValidators.MissingIn(assemblies);

        Assert.Empty(missing);
    }
}
