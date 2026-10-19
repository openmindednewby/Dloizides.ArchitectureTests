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
