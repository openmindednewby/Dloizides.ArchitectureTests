using System;
using Xunit;

namespace Dloizides.ArchitectureTests;

/// <summary>The house architecture rules; derive one test class per service.</summary>
/// <typeparam name="TWebMarker">Any type in the service's web assembly.</typeparam>
public abstract class HouseRules<TWebMarker>
{
    private static readonly Lazy<ServiceArchitecture> Service = new(() => ServiceArchitecture.Load(typeof(TWebMarker).Assembly));

    [Fact]
    public void DomainLayer_WhenDependenciesChecked_UsesNoOuterLayer() => Assert.Empty(Service.Value.ViolationsOf(ServiceLayer.Domain));

    [Fact]
    public void ApplicationLayer_WhenDependenciesChecked_UsesOnlyDomain() => Assert.Empty(Service.Value.ViolationsOf(ServiceLayer.Application));

    [Fact]
    public void InfrastructureLayer_WhenDependenciesChecked_DoesNotUseWeb() => Assert.Empty(Service.Value.ViolationsOf(ServiceLayer.Infrastructure));
}
