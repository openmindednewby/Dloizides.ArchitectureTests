using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Dloizides.ArchitectureTests;

/// <summary>The REPO-ONLY-1 rule; derive one test class per service.</summary>
public abstract class RepositoryOnlyDatabaseRules
{
    /// <summary>Returns the assemblies holding the service's classes.</summary>
    protected abstract IEnumerable<Assembly> ServiceAssemblies { get; }

    /// <summary>Returns the frozen classes that still take a DbContext, each with its reason; the list only shrinks.</summary>
    protected virtual IEnumerable<ArchitectureExemption> Exemptions => [];

    [Fact]
    public void DatabaseAccess_WhenClassesChecked_OnlyRepositoriesTakeADbContext() => Assert.Empty(ExemptionFilter.Unexempted(Outside(), Exemptions));

    [Fact]
    public void DatabaseAccessExemptions_WhenClassesChecked_EachStillTakesADbContext() => Assert.Empty(ExemptionFilter.Stale(Outside(), Exemptions));

    private IReadOnlyList<string> Outside() => DatabaseAccess.OutsideRepositoriesIn(ServiceAssemblies);
}
