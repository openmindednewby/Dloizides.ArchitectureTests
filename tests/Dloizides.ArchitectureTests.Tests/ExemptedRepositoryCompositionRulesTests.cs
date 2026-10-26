using System.Collections.Generic;
using System.Reflection;
using Mvc.Service;

namespace Dloizides.ArchitectureTests.Tests;

public sealed class ExemptedRepositoryCompositionRulesTests : RepositoryCompositionRules
{
    protected override IEnumerable<Assembly> ServiceAssemblies => [typeof(OrderRepository).Assembly];

    protected override IEnumerable<ArchitectureExemption> Exemptions =>
    [
        new(typeof(OrderRepository), "reads the customer row inside the order query"),
        new(typeof(LedgerStore), "the ledger wraps the account store"),
        new(typeof(CachedPriceRepository), "prices read the customer tier until the service owns it"),
    ];
}
