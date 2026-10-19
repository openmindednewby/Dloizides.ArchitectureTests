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

To skip a type on purpose, override `Exemptions` (tenant filter) or `RequestValidatorExemptions` (validator) with `new ArchitectureExemption(typeof(T), "why it is safe")`; the reason is required. An exempted type that gains a filter or validator fails the `...Exemptions_..._EachStill...` test, so a stale exemption cannot linger.
