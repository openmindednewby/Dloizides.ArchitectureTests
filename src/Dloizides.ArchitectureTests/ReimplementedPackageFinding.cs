namespace Dloizides.ArchitectureTests;

/// <summary>A service type that re-implements a shared package.</summary>
public sealed record ReimplementedPackageFinding(string TypeName, ReimplementedPackage Package)
{
    /// <summary>Names the type, the owning package and what to use instead.</summary>
    public override string ToString() => $"{TypeName} re-implements {Package.OwningPackage} ({Package.Pattern}): use {Package.Replacement}";
}
