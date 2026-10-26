using System.Collections.Generic;
using System.Reflection;
using Mvc.Service;

namespace Dloizides.ArchitectureTests.Tests;

public sealed class ExemptedControllerRouteRulesTests : ControllerRouteRules
{
    protected override IEnumerable<Assembly> ServiceAssemblies => [typeof(CasesController).Assembly];

    protected override IEnumerable<ArchitectureExemption> Exemptions =>
    [
        new(typeof(CaseArchiveController), "archive actions move into CasesController next"),
        new(typeof(CasesController), "shares v1/cases with the archive controller until the move"),
        new(typeof(IngestController), "ingest actions move into one controller next"),
        new(typeof(IngestJobController), "shares v1/ingest until the move"),
    ];
}
