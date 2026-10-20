using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Clean.Web;
using Reimplementing.Service;
using Reimplementing.Service.Security;
using Xunit;
using Xunit.Sdk;

namespace Dloizides.ArchitectureTests.Tests;

public sealed class ReimplementedPackageRulesTests
{
    [Fact]
    public void MissingIn_WithEveryMeasuredReimplementation_NamesEachTypeAndPattern()
    {
        var assembly = typeof(SwaggerSetup).Assembly;
        var expected = new[]
        {
            "Reimplementing.Service.ApiKeyAuthMiddleware api-key-auth",
            "Reimplementing.Service.LogStressEndpoint log-stress-endpoint",
            "Reimplementing.Service.LogStressRequest log-stress-endpoint",
            "Reimplementing.Service.LogStressRequestValidator log-stress-endpoint",
            "Reimplementing.Service.Security.RealmAuthorizationHandler realm-authorization",
            "Reimplementing.Service.SwaggerSetup swagger-oauth2-pkce",
            "Reimplementing.Service.SwashbuckleSwaggerSetup swagger-oauth2-pkce",
        };

        var findings = ReimplementedPackageRules.MissingIn(assembly);

        Assert.Equal(expected, findings.Select(finding => $"{finding.TypeName} {finding.Package.Pattern}"));
    }

    [Fact]
    public void MissingIn_WithLocalRealmHandler_NamesIdentityKeycloakAsOwner()
    {
        var assembly = typeof(RealmAuthorizationHandler).Assembly;
        var handler = typeof(RealmAuthorizationHandler).FullName!;

        var findings = ReimplementedPackageRules.MissingIn(assembly);

        Assert.Equal("Identity.Keycloak", findings.Single(finding => finding.TypeName == handler).Package.OwningPackage);
    }

    [Fact]
    public void MissingIn_WithCleanAssembly_ReportsNothing()
    {
        var assembly = typeof(CleanWebMarker).Assembly;

        var findings = ReimplementedPackageRules.MissingIn(assembly);

        Assert.Empty(findings);
    }

    [Fact]
    public void AssertNone_WithUnexemptedFinding_FailsNamingTheOwningPackage()
    {
        var assembly = typeof(LogStressEndpoint).Assembly;

        var failure = Record.Exception(() => ReimplementedPackageRules.AssertNone(assembly));

        Assert.IsAssignableFrom<XunitException>(failure);
        Assert.Contains("Reimplementing.Service.LogStressEndpoint re-implements Logging.Client (log-stress-endpoint)", failure.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AssertNone_WithEveryFindingExempted_Passes()
    {
        var assembly = typeof(SwaggerSetup).Assembly;
        var exemptions = new ArchitectureExemption[]
        {
            new(typeof(ApiKeyAuthMiddleware), "fixture"),
            new(typeof(LogStressEndpoint), "fixture"),
            new(typeof(RealmAuthorizationHandler), "fixture"),
            new(typeof(SwaggerSetup), "fixture"),
            new(typeof(SwashbuckleSwaggerSetup), "fixture"),
        };

        var failure = Record.Exception(() => ReimplementedPackageRules.AssertNone(assembly, exemptions));

        Assert.Null(failure);
    }

    [Fact]
    public void MissingIn_WithSwashbuckleOAuthFlows_NamesSwaggerPattern()
    {
        var assembly = typeof(SwashbuckleSwaggerSetup).Assembly;
        var setup = typeof(SwashbuckleSwaggerSetup).FullName!;

        var findings = ReimplementedPackageRules.MissingIn(assembly);

        Assert.Equal("swagger-oauth2-pkce", findings.Single(finding => finding.TypeName == setup).Package.Pattern);
    }

    [Fact]
    public void AssertNone_WithEndpointExempted_CoversItsRequestAndValidator()
    {
        var assembly = typeof(LogStressEndpoint).Assembly;
        var exemptions = new ArchitectureExemption[]
        {
            new(typeof(ApiKeyAuthMiddleware), "fixture"),
            new(typeof(LogStressEndpoint), "fixture"),
            new(typeof(RealmAuthorizationHandler), "fixture"),
            new(typeof(SwaggerSetup), "fixture"),
            new(typeof(SwashbuckleSwaggerSetup), "fixture"),
        };

        var failure = Record.Exception(() => ReimplementedPackageRules.AssertNone(assembly, exemptions));

        Assert.Null(failure);
    }

    [Fact]
    public void MissingIn_WithAssemblyWithoutLocation_ThrowsNamingTheAssembly()
    {
        var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("InMemory.Fixture"), AssemblyBuilderAccess.Run);

        var failure = Record.Exception(() => ReimplementedPackageRules.MissingIn(assembly));

        Assert.IsType<ArgumentException>(failure);
        Assert.Contains("InMemory.Fixture has no file location", failure.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AssertNone_WithStaleExemption_FailsNamingTheExemptedType()
    {
        var assembly = typeof(CleanWebMarker).Assembly;
        var exemption = new ArchitectureExemption(typeof(CleanWebMarker), "fixture");

        var failure = Record.Exception(() => ReimplementedPackageRules.AssertNone(assembly, exemption));

        Assert.IsAssignableFrom<XunitException>(failure);
        Assert.Contains("Clean.Web.CleanWebMarker is exempted but re-implements no package any more", failure.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void All_WhenRegistryChecked_EachPatternHasASignal()
    {
        var patterns = ReimplementedPackages.All;

        var silent = patterns.Where(package => package.DeclaredTypeNames.Count == 0 && package.ReferencedTypes.Count == 0).Select(package => package.Pattern);

        Assert.Empty(silent);
    }
}
