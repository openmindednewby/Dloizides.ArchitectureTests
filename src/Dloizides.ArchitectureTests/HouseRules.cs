using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Dloizides.ArchitectureTests;

/// <summary>The house architecture rules; derive one test class per service.</summary>
public abstract class HouseRules<TWebMarker>
{
    private static readonly Lazy<ServiceArchitecture> Service = new(() => ServiceArchitecture.Load(typeof(TWebMarker).Assembly));

    [Fact]
    public void DomainLayer_WhenDependenciesChecked_UsesNoOuterLayer() => Assert.Empty(Service.Value.ViolationsOf(ServiceLayer.Domain));

    [Fact]
    public void ApplicationLayer_WhenDependenciesChecked_UsesOnlyDomain() => Assert.Empty(Service.Value.ViolationsOf(ServiceLayer.Application));

    [Fact]
    public void InfrastructureLayer_WhenDependenciesChecked_DoesNotUseWeb() => Assert.Empty(Service.Value.ViolationsOf(ServiceLayer.Infrastructure));

    [Fact]
    public void Requests_WhenValidatorsChecked_EachHasAValidator() => Assert.Empty(ExemptionFilter.Unexempted(MissingValidators(), RequestValidatorExemptions));

    [Fact]
    public void RequestValidatorExemptions_WhenValidatorsChecked_EachStillLacksAValidator() => Assert.Empty(ExemptionFilter.Stale(MissingValidators(), RequestValidatorExemptions));

    /// <summary>Returns the endpoint requests left without a validator on purpose, each with its reason.</summary>
    protected virtual IEnumerable<ArchitectureExemption> RequestValidatorExemptions => [];

    private static IReadOnlyList<string> MissingValidators() => RequestValidators.MissingIn(Service.Value.Layers.Values.SelectMany(layer => layer));
}
