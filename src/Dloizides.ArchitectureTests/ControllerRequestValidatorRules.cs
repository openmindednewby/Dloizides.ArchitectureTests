using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Dloizides.ArchitectureTests;

/// <summary>The MVC validator-per-request rule; derive one test class per service.</summary>
public abstract class ControllerRequestValidatorRules
{
    /// <summary>Returns the assemblies holding the service's controllers, requests and validators.</summary>
    protected abstract IEnumerable<Assembly> ServiceAssemblies { get; }

    /// <summary>Returns the action requests left without a validator on purpose, each with its reason.</summary>
    protected virtual IEnumerable<ArchitectureExemption> Exemptions => [];

    [Fact]
    public void ControllerRequests_WhenValidatorsChecked_EachHasAValidator() => Assert.Empty(ExemptionFilter.Unexempted(Missing(), Exemptions));

    [Fact]
    public void ControllerRequestValidatorExemptions_WhenValidatorsChecked_EachStillLacksAValidator() => Assert.Empty(ExemptionFilter.Stale(Missing(), Exemptions));

    private IReadOnlyList<string> Missing() => ControllerRequestValidators.MissingIn(ServiceAssemblies);
}
