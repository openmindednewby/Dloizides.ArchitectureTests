using System.Collections.Generic;
using Clean.Core;
using Clean.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Dloizides.ArchitectureTests.Tests;

public sealed class ExemptedTenantFilterRulesTests : TenantFilterRules<CleanWebMarker>
{
    protected override IEnumerable<ArchitectureExemption> Exemptions => [new(typeof(Venue), "venues are listed across tenants on the public map")];

    protected override IReadOnlyModel CreateModel()
    {
        var builder = new ModelBuilder();
        builder.Entity<Venue>().Property(venue => venue.TenantId);
        return builder.Model;
    }
}
