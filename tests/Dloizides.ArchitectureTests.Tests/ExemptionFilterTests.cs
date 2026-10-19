using System;
using Clean.Core;
using Xunit;

namespace Dloizides.ArchitectureTests.Tests;

public sealed class ExemptionFilterTests
{
    private static readonly ArchitectureExemption VenueExemption = new(typeof(Venue), "read across tenants by design");

    [Fact]
    public void Unexempted_WithExemptedFinding_DropsOnlyThatFinding()
    {
        string[] findings = [typeof(Venue).FullName!, typeof(Event).FullName!];

        var unexempted = ExemptionFilter.Unexempted(findings, [VenueExemption]);

        Assert.Equal(new[] { typeof(Event).FullName! }, unexempted);
    }

    [Fact]
    public void Stale_WhenExemptedTypeNoLongerBreaksTheRule_NamesTheExemption()
    {
        string[] findings = [typeof(Event).FullName!];

        var stale = ExemptionFilter.Stale(findings, [VenueExemption]);

        Assert.Equal(new[] { typeof(Venue).FullName! }, stale);
    }

    [Fact]
    public void Stale_WhenExemptedTypeStillBreaksTheRule_ReportsNothing()
    {
        string[] findings = [typeof(Venue).FullName!];

        var stale = ExemptionFilter.Stale(findings, [VenueExemption]);

        Assert.Empty(stale);
    }

    [Fact]
    public void Constructor_WithBlankReason_Throws()
    {
        var reason = " ";

        var create = () => new ArchitectureExemption(typeof(Venue), reason);

        Assert.Throws<ArgumentException>(create);
    }
}
