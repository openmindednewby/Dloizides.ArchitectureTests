using System.Collections.Generic;
using System.Reflection;
using Mvc.Service;

namespace Dloizides.ArchitectureTests.Tests;

public sealed class ExemptedRepositoryOnlyDatabaseRulesTests : RepositoryOnlyDatabaseRules
{
    protected override IEnumerable<Assembly> ServiceAssemblies => [typeof(ShopDbContext).Assembly];

    protected override IEnumerable<ArchitectureExemption> Exemptions =>
    [
        new(typeof(InvoiceService), "frozen before REPO-ONLY-1"),
        new(typeof(InvoicesController), "frozen before REPO-ONLY-1"),
        new(typeof(InvoiceCleanupJob), "frozen before REPO-ONLY-1"),
    ];
}
