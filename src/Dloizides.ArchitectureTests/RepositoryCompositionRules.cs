using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Dloizides.ArchitectureTests;

/// <summary>The C2 repository composition rule; derive one test class per service.</summary>
public abstract class RepositoryCompositionRules
{
    /// <summary>Returns the assemblies holding the service's repositories.</summary>
    protected abstract IEnumerable<Assembly> ServiceAssemblies { get; }

    /// <summary>Returns the repositories allowed to take another repository, each with its reason.</summary>
    protected virtual IEnumerable<ArchitectureExemption> Exemptions => [];

    [Fact]
    public void Repositories_WhenConstructorsChecked_TakeNoOtherRepository() => Assert.Empty(ExemptionFilter.Unexempted(Combining(), Exemptions));

    [Fact]
    public void RepositoryCompositionExemptions_WhenConstructorsChecked_EachStillTakesARepository() => Assert.Empty(ExemptionFilter.Stale(Combining(), Exemptions));

    private IReadOnlyList<string> Combining() => RepositoryDependencies.CombiningIn(ServiceAssemblies);
}
