// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

namespace BeneficialStrategies.Iso20022.Framework;

/// <summary>The attributes that describe the interfaces of the model, for what implements them (a generator, a reader).</summary>
public class ModelMetadataAttributeTests
{
    [IsoDisplayName("Group Header")]
    private interface IMyHeader
    {
        [IsoList(IsoListKind.SimpleValue)]
        IEnumerable<string> Names { get; }

        [IsoList(IsoListKind.Value)]
        IEnumerable<object> Items { get; }
    }

    [Fact]
    public void AnInterfaceCarriesItsIsoName()
    {
        var name = typeof(IMyHeader).GetCustomAttributes(typeof(IsoDisplayNameAttribute), false).Cast<IsoDisplayNameAttribute>().Single();

        Assert.Equal("Group Header", name.Name);
    }

    [Fact]
    public void AMemberThatMayRepeat_CarriesTheKindOfListThatHoldsItsItems()
    {
        var names = typeof(IMyHeader).GetProperty(nameof(IMyHeader.Names))!.GetCustomAttributes(typeof(IsoListAttribute), false).Cast<IsoListAttribute>().Single();
        var items = typeof(IMyHeader).GetProperty(nameof(IMyHeader.Items))!.GetCustomAttributes(typeof(IsoListAttribute), false).Cast<IsoListAttribute>().Single();

        Assert.Equal(IsoListKind.SimpleValue, names.Kind);
        Assert.Equal(IsoListKind.Value, items.Kind);
    }

    [Fact]
    public void TheDisplayNameIsOnlyForInterfaces_AndTheListKindOnlyForMembers()
    {
        Assert.Equal(AttributeTargets.Interface, typeof(IsoDisplayNameAttribute).GetCustomAttributes(typeof(AttributeUsageAttribute), false).Cast<AttributeUsageAttribute>().Single().ValidOn);
        Assert.Equal(AttributeTargets.Property, typeof(IsoListAttribute).GetCustomAttributes(typeof(AttributeUsageAttribute), false).Cast<AttributeUsageAttribute>().Single().ValidOn);
    }
}
