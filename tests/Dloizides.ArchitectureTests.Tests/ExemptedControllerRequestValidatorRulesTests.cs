using System.Collections.Generic;
using System.Reflection;
using Mvc.Service;

namespace Dloizides.ArchitectureTests.Tests;

public sealed class ExemptedControllerRequestValidatorRulesTests : ControllerRequestValidatorRules
{
    protected override IEnumerable<Assembly> ServiceAssemblies => [typeof(CasesController).Assembly];

    protected override IEnumerable<ArchitectureExemption> Exemptions =>
    [
        new(typeof(CloseCaseRequest), "closing only records a free-text reason"),
        new(typeof(CaseSearchQuery), "the search term is matched as a literal"),
    ];
}
