using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Dloizides.ArchitectureTests;

/// <summary>The C3 one-controller-per-route rule; derive one test class per service.</summary>
public abstract class ControllerRouteRules
{
    /// <summary>Returns the assemblies holding the service's controllers.</summary>
    protected abstract IEnumerable<Assembly> ServiceAssemblies { get; }

    /// <summary>Returns the controllers allowed to share a route template, each with its reason.</summary>
    protected virtual IEnumerable<ArchitectureExemption> Exemptions => [];

    [Fact]
    public void Controllers_WhenRoutesChecked_EachRouteHasOneController() => Assert.Empty(ExemptionFilter.Unexempted(Shared(), Exemptions));

    [Fact]
    public void ControllerRouteExemptions_WhenRoutesChecked_EachStillSharesARoute() => Assert.Empty(ExemptionFilter.Stale(Shared(), Exemptions));

    private IReadOnlyList<string> Shared() => ControllerRoutes.SharedIn(ServiceAssemblies);
}
