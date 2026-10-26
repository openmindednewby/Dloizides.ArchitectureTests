# Dloizides.ArchitectureTests

The house architecture rules, run as xUnit tests over [ArchUnitNET](https://github.com/TNG/ArchUnitNET).

| Rule | Fails when |
|---|---|
| PAT-LAYER-1 "Clean Architecture layer direction" | a Domain type uses an Application, Infrastructure or Web type; an Application type uses an Infrastructure or Web type; an Infrastructure type uses a Web type |

## Adopt

```xml
<PackageReference Include="Dloizides.ArchitectureTests" Version="1.0.0" />
```

Add one class to a test project that references every layer of the service:

```csharp
public sealed class HouseArchitectureTests : HouseRules<Program>;
```

The type argument is any type in the service's web assembly. Layers are found by assembly name next to
that assembly: the web assembly is `<Prefix>.Web` or `<Prefix>.API`, and the others are
`<Prefix>.Domain` or `<Prefix>.Core`, `<Prefix>.Application` or `<Prefix>.UseCases`, and
`<Prefix>.Infrastructure`. A missing layer is skipped. Other assemblies (`<Prefix>.Contracts`,
`<Prefix>.ServiceDefaults`) are not checked. The architecture is loaded once per test class type.

Built for xUnit 2.9.

## Validator per request and tenant query filters

`HouseRules<T>` also fails when a type used as the request of an endpoint (`Endpoint<TRequest, ...>`) in the service's layer assemblies has a property not named `*Id` and no FastEndpoints `Validator<T>` (PAT-REVIEW-15). A bare FluentValidation `AbstractValidator<T>` does not count: FastEndpoints only registers it with `IncludeAbstractValidators = true`, so it never runs. For tenant data, add `public sealed class TenantFilterTests : TenantFilterRules<WebMarker> { protected override IReadOnlyModel CreateModel() => ...context.Model; }`: it fails for every root entity declared in the service's own layer assemblies (package entities are skipped) with a `TenantId` property and no query filter (PAT-REVIEW-12).

For an MVC service, add `public sealed class ControllerValidatorTests : ControllerRequestValidatorRules { protected override IEnumerable<Assembly> ServiceAssemblies => [typeof(Program).Assembly]; }`: it fails for every complex parameter of a public controller action (not `[NonAction]`, not `[FromRoute]`/`[FromHeader]`/`[FromServices]`) declared in those assemblies with a property not named `*Id` and no concrete class implementing FluentValidation `IValidator<T>`, the set `AddValidatorsFromAssembly` registers and a per-argument action filter resolves. A service built as one assembly without `.Domain`/`.Web` suffixes overrides `ServiceAssemblies` on `TenantFilterRules<T>` the same way.

To skip a type on purpose, override `Exemptions` (tenant filter, MVC validator) or `RequestValidatorExemptions` (validator) with `new ArchitectureExemption(typeof(T), "why it is safe")`; the reason is required. An exempted type that gains a filter or validator fails the `...Exemptions_..._EachStill...` test, so a stale exemption cannot linger.

## Repository composition and one controller per route

Both rules are opt-in, so adopting a new package version never turns a service red on its own.

`public sealed class RepositoryTests : RepositoryCompositionRules { protected override IEnumerable<Assembly> ServiceAssemblies => [typeof(Program).Assembly]; }` fails for every concrete class named `*Repository` (or implementing an interface named `*Repository`, generic ones included) whose public constructor takes another repository (C2 "A repository never takes another repository; combining is a service's job"). A decorator that takes only the repository interface it implements (`CachedOrderRepository : IOrderRepository` taking `IOrderRepository`) passes. A service that needs two repositories takes both.

`public sealed class RouteTests : ControllerRouteRules { protected override IEnumerable<Assembly> ServiceAssemblies => [typeof(Program).Assembly]; }` fails for every MVC controller (derives from `ControllerBase`) whose class-level `[Route]` template another controller also declares (C3 "One controller per route prefix; new actions on a resource join its controller"). `[controller]` is replaced by the class name minus `Controller`; the comparison ignores case and leading/trailing `/`. A `[Route]` inherited from a base controller counts. A FastEndpoints service has no controllers, so the rule passes with nothing to check. Templates are compared whole: `api/parks` and `api/parks/{id}/reports` do not clash, and `[area]` is not substituted.

Both take `Exemptions` as a baseline of known offenders (`new ArchitectureExemption(typeof(T), "why")`); an exempted type that stops offending fails the `...Exemptions_..._EachStill...` test.

## Re-implemented packages

`ReimplementedPackageRules.AssertNone(typeof(Program).Assembly)` fails for every type in that assembly that re-implements a shared package listed in `ReimplementedPackages.All` (QUAL-ADOPT-1 "Fail lint/arch tests when an app re-implements an existing package"): a local `RealmAuthorizationHandler`/`RealmAuthorizationExtensions` (use `Identity.Keycloak`), an own `ApiKeyAuthMiddleware`/`ApiKeyAuthenticationHandler` (use `Dloizides.ApiKeys`), a method body that builds an NSwag `OpenApiOAuthFlow` (the Swagger OAuth2/PKCE block, owned by `ApiDocs.AspNetCore`), and the `LogStress` debug endpoint (delete it). `ReimplementedPackageRules.MissingIn(assembly)` returns the findings. Pass `new ArchitectureExemption(typeof(T), "why")` for a type kept on purpose; an exemption that matches nothing fails too.

```csharp
[Fact]
public void Service_WhenPackagesChecked_ReimplementsNone() => ReimplementedPackageRules.AssertNone(typeof(Program).Assembly);
```
