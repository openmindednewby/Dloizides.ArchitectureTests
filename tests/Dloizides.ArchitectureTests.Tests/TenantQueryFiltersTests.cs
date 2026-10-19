using System;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dloizides.ArchitectureTests.Tests;

public sealed class TenantQueryFiltersTests
{
    [Fact]
    public void MissingIn_WithUnfilteredTenantEntity_NamesTheEntity()
    {
        var builder = new ModelBuilder();
        builder.Entity<FilteredShop>(shop => shop.Property(s => s.TenantId)).Entity<FilteredShop>().HasQueryFilter(s => s.TenantId != Guid.Empty);
        builder.Entity<UnfilteredShop>().Property(s => s.TenantId);
        builder.Entity<Country>().Property(c => c.Id);
        var expected = typeof(UnfilteredShop).FullName!;

        var missing = TenantQueryFilters.MissingIn(builder.Model);

        Assert.Equal(new[] { expected }, missing);
    }

    [Fact]
    public void MissingIn_WithEveryTenantEntityFiltered_ReportsNothing()
    {
        var builder = new ModelBuilder();
        builder.Entity<FilteredShop>().Property(s => s.TenantId);
        builder.Entity<FilteredShop>().HasQueryFilter(s => s.TenantId != Guid.Empty);

        var missing = TenantQueryFilters.MissingIn(builder.Model);

        Assert.Empty(missing);
    }

    public sealed class FilteredShop
    {
        public int Id { get; set; }

        public Guid TenantId { get; set; }
    }

    public sealed class UnfilteredShop
    {
        public int Id { get; set; }

        public Guid TenantId { get; set; }
    }

    public sealed class Country
    {
        public int Id { get; set; }
    }
}
