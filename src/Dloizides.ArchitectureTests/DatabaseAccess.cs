using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Dloizides.ArchitectureTests;

/// <summary>REPO-ONLY-1 "Only repositories touch the database".</summary>
public static class DatabaseAccess
{
    private const string DbContextName = "Microsoft.EntityFrameworkCore.DbContext";
    private const string ContextFactoryName = "Microsoft.EntityFrameworkCore.IDbContextFactory`1";
    private const BindingFlags DeclaredMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static readonly string[] DataAccessSuffixes = ["Repository", "Store"];

    private static readonly string[] DataAccessBaseTypes =
    [
        DbContextName,
        "Microsoft.EntityFrameworkCore.Migrations.Migration",
        "Microsoft.EntityFrameworkCore.Infrastructure.ModelSnapshot",
    ];

    private static readonly string[] DataAccessInterfaces =
    [
        "Microsoft.EntityFrameworkCore.IEntityTypeConfiguration`1",
        "Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory`1",
    ];

    /// <summary>Names every class, not named *Repository or *Store, whose constructor or field takes a DbContext.</summary>
    public static IReadOnlyList<string> OutsideRepositoriesIn(IEnumerable<Assembly> assemblies) =>
        ServiceTypes.Load(assemblies)
            .Where(type => type.IsClass && !IsCompilerGenerated(type) && !MayTouchDatabase(type))
            .Where(TakesADatabaseHandle)
            .Select(type => type.FullName ?? type.Name)
            .Distinct()
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

    private static bool TakesADatabaseHandle(Type type) =>
        type.GetConstructors(DeclaredMembers).SelectMany(constructor => constructor.GetParameters()).Any(parameter => IsDatabaseHandle(parameter.ParameterType))
        || type.GetFields(DeclaredMembers).Any(field => IsDatabaseHandle(field.FieldType));

    private static bool IsDatabaseHandle(Type type) =>
        DerivesFrom(type, DbContextName)
        || (type.IsGenericType && string.Equals(type.GetGenericTypeDefinition().FullName, ContextFactoryName, StringComparison.Ordinal));

    private static bool MayTouchDatabase(Type type) =>
        Array.Exists(DataAccessSuffixes, suffix => BareName(type).EndsWith(suffix, StringComparison.Ordinal))
        || Array.Exists(DataAccessBaseTypes, baseType => DerivesFrom(type, baseType))
        || type.GetInterfaces().Any(contract => contract.IsGenericType && Array.Exists(DataAccessInterfaces, name => string.Equals(contract.GetGenericTypeDefinition().FullName, name, StringComparison.Ordinal)));

    private static bool DerivesFrom(Type type, string baseTypeName)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (string.Equals(current.FullName, baseTypeName, StringComparison.Ordinal))
                return true;
        }
        return false;
    }

    private static bool IsCompilerGenerated(Type type) =>
        type.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false) || (type.DeclaringType is not null && IsCompilerGenerated(type.DeclaringType));

    private static string BareName(Type type)
    {
        var arity = type.Name.IndexOf('`', StringComparison.Ordinal);
        return arity < 0 ? type.Name : type.Name[..arity];
    }
}
