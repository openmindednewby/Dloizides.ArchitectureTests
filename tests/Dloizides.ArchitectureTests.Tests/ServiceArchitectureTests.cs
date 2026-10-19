using System;
using System.Linq;
using Clean.Web;
using Dirty.Web;
using Xunit;

namespace Dloizides.ArchitectureTests.Tests;

public sealed class ServiceArchitectureTests
{
    [Theory]
    [InlineData(ServiceLayer.Domain, "Dirty.Domain.Order")]
    [InlineData(ServiceLayer.Application, "Dirty.Application.OrderHandler")]
    [InlineData(ServiceLayer.Infrastructure, "Dirty.Infrastructure.OrderStore")]
    public void ViolationsOf_WithPlantedOuterLayerUse_ReportsTheType(ServiceLayer layer, string offendingType)
    {
        var service = ServiceArchitecture.Load(typeof(DirtyWebMarker).Assembly);

        var violations = service.ViolationsOf(layer);

        Assert.Contains(violations, violation => violation.Contains(offendingType, StringComparison.Ordinal));
    }

    [Theory]
    [InlineData(ServiceLayer.Domain)]
    [InlineData(ServiceLayer.Application)]
    [InlineData(ServiceLayer.Infrastructure)]
    [InlineData(ServiceLayer.Web)]
    public void ViolationsOf_WithCleanLayers_ReportsNothing(ServiceLayer layer)
    {
        var service = ServiceArchitecture.Load(typeof(CleanWebMarker).Assembly);

        var violations = service.ViolationsOf(layer);

        Assert.Empty(violations);
    }

    [Fact]
    public void Load_WithCoreAndUseCasesNames_MapsThemToDomainAndApplication()
    {
        var service = ServiceArchitecture.Load(typeof(CleanWebMarker).Assembly);

        var layers = service.Layers.ToDictionary(pair => pair.Key, pair => pair.Value.Single().GetName().Name);

        Assert.Equal("Clean.Core", layers[ServiceLayer.Domain]);
        Assert.Equal("Clean.UseCases", layers[ServiceLayer.Application]);
        Assert.False(layers.ContainsKey(ServiceLayer.Infrastructure));
    }

    [Fact]
    public void Load_WithoutWebSuffix_ThrowsArgumentException()
    {
        var notWeb = typeof(object).Assembly;

        var load = () => ServiceArchitecture.Load(notWeb);

        Assert.Throws<ArgumentException>(load);
    }
}
