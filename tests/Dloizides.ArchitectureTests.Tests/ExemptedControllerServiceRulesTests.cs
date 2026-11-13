using System.Collections.Generic;
using System.Reflection;
using Mvc.Service;

namespace Dloizides.ArchitectureTests.Tests;

public sealed class ExemptedControllerServiceRulesTests : ControllerServiceRules
{
    protected override IEnumerable<Assembly> ServiceAssemblies => [typeof(CustomersController).Assembly];

    protected override IEnumerable<ArchitectureExemption> Exemptions =>
    [
        new(typeof(CustomersController), "reads customers until the customer service exists"),
        new(typeof(ProductListEndpoint), "lists products until the catalogue service exists"),
        new(typeof(OrderExportEndpoint), "exports orders until the export service exists"),
    ];
}
