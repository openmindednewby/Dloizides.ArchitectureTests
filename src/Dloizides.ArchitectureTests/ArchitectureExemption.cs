using System;

namespace Dloizides.ArchitectureTests;

/// <summary>A type a rule skips on purpose, with the reason it is safe to skip.</summary>
public sealed record ArchitectureExemption
{
    /// <summary>Exempts type from a rule; reason must say why breaking the rule is safe.</summary>
    public ArchitectureExemption(Type type, string reason)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        Type = type;
        Reason = reason;
    }

    /// <summary>The exempted type.</summary>
    public Type Type { get; }

    /// <summary>Why the type may break the rule.</summary>
    public string Reason { get; }

    /// <summary>The full name a rule reports the type under.</summary>
    public string Name => Type.FullName ?? Type.Name;
}
