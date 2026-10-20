using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Mvc.Service;

namespace Dloizides.ArchitectureTests.Tests;

public sealed class SingleAssemblyTenantFilterRulesTests : TenantFilterRules<CasesController>
{
    protected override IEnumerable<Assembly> ServiceAssemblies => [typeof(Account).Assembly];

    protected override IEnumerable<ArchitectureExemption> Exemptions => [new(typeof(Account), "accounts are listed across tenants by the operator console")];

    protected override IReadOnlyModel CreateModel()
    {
        var builder = new ModelBuilder();
        builder.Entity<Account>().Property(account => account.TenantId);
        return builder.Model;
    }
}
