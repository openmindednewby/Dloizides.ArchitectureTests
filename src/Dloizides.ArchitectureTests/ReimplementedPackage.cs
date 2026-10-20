using System.Collections.Generic;

namespace Dloizides.ArchitectureTests;

/// <summary>A pattern a service writes by hand although a shared package provides it.</summary>
/// <param name="DeclaredTypeNames">Type names that mark the pattern.</param>
/// <param name="ReferencedTypes">Full names of types whose members a method body uses to build it.</param>
public sealed record ReimplementedPackage(string Pattern, string OwningPackage, string Replacement, IReadOnlyList<string> DeclaredTypeNames, IReadOnlyList<string> ReferencedTypes);
