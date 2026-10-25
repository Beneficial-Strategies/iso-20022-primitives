// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

namespace BeneficialStrategies.Iso20022.SimpleTypes;

public class ISOMonthTests : SimpleValueCompositeIntContractTests<ISOMonth>
{
    protected override int ValidNativeSample => 3;
    protected override string ValidWireText => "03";
    protected override int OutOfRangeNativeSample => 13;

    [Theory]
    [InlineData(1, "01")]
    [InlineData(9, "09")]
    [InlineData(12, "12")]
    public void AMonth_IsWrittenWithTwoDigits(int month, string text)
    {
        Assert.Equal(text, new ISOMonth(month).ToString());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    [InlineData(-1)]
    public void ANumberThatIsNotAMonth_ThrowsFormatException(int month)
    {
        Assert.Throws<Iso20022FormatException>(() => new ISOMonth(month));
    }
}
