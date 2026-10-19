using System.Linq;
using Clean.Web;
using Dirty.Web;
using Xunit;

namespace Dloizides.ArchitectureTests.Tests;

public sealed class RequestValidatorsTests
{
    [Fact]
    public void MissingIn_WithRequestLackingValidator_NamesTheRequest()
    {
        var assemblies = ServiceArchitecture.Load(typeof(DirtyWebMarker).Assembly).Layers.Values.SelectMany(layer => layer);
        var expected = typeof(PlaceOrderRequest).FullName!;

        var missing = RequestValidators.MissingIn(assemblies);

        Assert.Equal(new[] { expected }, missing);
    }

    [Fact]
    public void MissingIn_WithEveryRequestValidated_ReportsNothing()
    {
        var assemblies = ServiceArchitecture.Load(typeof(CleanWebMarker).Assembly).Layers.Values.SelectMany(layer => layer);

        var missing = RequestValidators.MissingIn(assemblies);

        Assert.Empty(missing);
    }
}
