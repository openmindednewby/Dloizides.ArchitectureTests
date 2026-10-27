using System;
using Clean.Web;
using Mvc.Service;
using Xunit;

namespace Dloizides.ArchitectureTests.Tests;

public sealed class DatabaseAccessTests
{
    [Fact]
    public void OutsideRepositoriesIn_WithClassesTakingADbContext_NamesOnlyTheNonRepositories()
    {
        var assemblies = new[] { typeof(ShopDbContext).Assembly };
        var expected = new[] { typeof(InvoiceCleanupJob).FullName!, typeof(InvoiceService).FullName!, typeof(InvoicesController).FullName! };

        var outside = DatabaseAccess.OutsideRepositoriesIn(assemblies);

        Assert.Equal(expected, outside);
    }

    [Theory]
    [InlineData(typeof(InvoiceRepository))]
    [InlineData(typeof(InvoiceStore))]
    [InlineData(typeof(ArchiveDbContext))]
    [InlineData(typeof(InvoiceConfiguration))]
    [InlineData(typeof(ShopDbContextFactory))]
    [InlineData(typeof(ShopModelSnapshot))]
    [InlineData(typeof(AddInvoices))]
    public void OutsideRepositoriesIn_WithDataAccessClass_DoesNotNameIt(Type dataAccess)
    {
        var assemblies = new[] { dataAccess.Assembly };

        var outside = DatabaseAccess.OutsideRepositoriesIn(assemblies);

        Assert.DoesNotContain(dataAccess.FullName!, outside);
    }

    [Fact]
    public void OutsideRepositoriesIn_WithoutDbContext_ReportsNothing()
    {
        var assemblies = new[] { typeof(CleanWebMarker).Assembly };

        var outside = DatabaseAccess.OutsideRepositoriesIn(assemblies);

        Assert.Empty(outside);
    }
}
