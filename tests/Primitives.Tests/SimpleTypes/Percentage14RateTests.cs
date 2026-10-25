// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

namespace BeneficialStrategies.Iso20022.SimpleTypes;

public class Percentage14RateTests : SimpleValueXsdNumericContractTests<Percentage14Rate, decimal>
{
    protected override decimal ValidNativeSample => 1.5m;

    [Fact]
    public void ThirteenFractionDigits_AreAccepted()
    {
        Assert.Equal(0.1234567890123m, new Percentage14Rate(0.1234567890123m).Value);
    }

    [Fact]
    public void FourteenDigitsInTotal_AreAccepted()
    {
        Assert.Equal(12345678901234m, new Percentage14Rate(12345678901234m).Value);
    }

    [Fact]
    public void ExcessiveTotalDigits_ThrowsFormatException()
    {
        Assert.Throws<Iso20022FormatException>(() => new Percentage14Rate(123456789012345m));
    }

    [Fact]
    public void ExcessiveFractionDigits_ThrowsFormatException()
    {
        Assert.Throws<Iso20022FormatException>(() => new Percentage14Rate(0.12345678901234m));
    }
}
