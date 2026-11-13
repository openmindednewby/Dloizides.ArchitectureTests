using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Dloizides.ArchitectureTests;

/// <summary>The CONTROLLER-SERVICE-1 rule; derive one test class per service.</summary>
public abstract class ControllerServiceRules
{
    /// <summary>Returns the assemblies holding the service's controllers and endpoints.</summary>
    protected abstract IEnumerable<Assembly> ServiceAssemblies { get; }

    /// <summary>Returns the controllers and endpoints allowed to take a repository, each with its reason.</summary>
    protected virtual IEnumerable<ArchitectureExemption> Exemptions => [];

    [Fact]
    public void Controllers_WhenConstructorsChecked_TakeNoRepository() => Assert.Empty(ExemptionFilter.Unexempted(TakingRepository(), Exemptions));

    [Fact]
    public void ControllerServiceExemptions_WhenConstructorsChecked_EachStillTakesARepository() => Assert.Empty(ExemptionFilter.Stale(TakingRepository(), Exemptions));

    private IReadOnlyList<string> TakingRepository() => ControllerDependencies.TakingRepositoryIn(ServiceAssemblies);
}
