// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

namespace BeneficialStrategies.Iso20022.Framework;

/// <summary>Which list type holds the items of a member that may repeat.</summary>
public enum IsoListKind
{
    /// <summary>A <see cref="ValueList{T}"/>: a list of objects and of values that carry their own checks, compared by its items.</summary>
    Value = 0,

    /// <summary>A <see cref="SimpleValueList{T}"/>: a list of simple values, compared by its items.</summary>
    SimpleValue = 1,
}

/// <summary>
/// On a member of an interface that is declared as <c>IEnumerable&lt;T&gt;</c>: the kind of list that an implementation holds its items in,
/// so that two objects with the same items are equal, as the records are.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class IsoListAttribute : Attribute
{
    /// <summary>The kind of list that holds the items.</summary>
    public IsoListKind Kind { get; }

    /// <summary>Marks a member that may repeat with the kind of list that holds its items.</summary>
    /// <param name="kind">The kind of list.</param>
    public IsoListAttribute(IsoListKind kind) => Kind = kind;
}
