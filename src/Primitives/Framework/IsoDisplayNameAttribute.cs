// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

namespace BeneficialStrategies.Iso20022.Framework;

/// <summary>
/// The name of a type as the ISO 20022 repository gives it (for example "Group Header"). <see cref="System.ComponentModel.DisplayNameAttribute"/>
/// is not valid on an interface, so the interfaces of the model carry this one, and what implements them can carry the other.
/// </summary>
[AttributeUsage(AttributeTargets.Interface)]
public sealed class IsoDisplayNameAttribute : Attribute
{
    /// <summary>The name of the type in the ISO 20022 repository.</summary>
    public string Name { get; }

    /// <summary>Marks an interface with the ISO name of its type.</summary>
    /// <param name="name">The name of the type in the ISO 20022 repository.</param>
    public IsoDisplayNameAttribute(string name) => Name = name;
}
