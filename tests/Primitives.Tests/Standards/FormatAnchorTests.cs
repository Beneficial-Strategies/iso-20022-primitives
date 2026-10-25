// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

using BeneficialStrategies.Iso20022.SimpleTypes;

namespace BeneficialStrategies.Iso20022.Standards;

/// <summary>
/// An XML Schema pattern must match the whole text. In .NET a pattern ending in <c>$</c> also matches
/// before a final line feed, so <c>"5411\n"</c> would pass <c>^[0-9]{4}$</c>. The patterns of the
/// types end in <c>\z</c>; these tests keep it so. Found by comparing every type with ISO's XSDs.
/// </summary>
public class FormatAnchorTests
{
    [Fact]
    public void NoPatternOfAType_EndsWithADollar()
    {
        var offenders = typeof(ISODateTime).Assembly.GetTypes()
            .Where(t => t.IsPublic)
            .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.IsLiteral && f.FieldType == typeof(string) && f.Name.EndsWith("Pattern", StringComparison.Ordinal))
                .Select(f => (Type: t.Name, Field: f.Name, Value: (string)f.GetRawConstantValue()!)))
            .Where(x => x.Value.EndsWith("$", StringComparison.Ordinal) && !x.Value.EndsWith(@"\$", StringComparison.Ordinal))
            .Select(x => $"{x.Type}.{x.Field}")
            .ToList();
        Assert.Empty(offenders);
    }

    [Theory]
    [InlineData("ActiveCurrencyCode", "EUR\n")]
    [InlineData("CountryCode", "DE\n")]
    [InlineData("ISO18245MerchantCategoryCode", "5411\n")]
    [InlineData("IBAN2007Identifier", "GB82WEST12345698765432\n")]
    [InlineData("LEIIdentifier", "5493001KJTIIGC8Y1R12\n")]
    [InlineData("ISINIdentifier", "US0378331005\n")]
    [InlineData("BICIdentifier", "DEUTDEFF\n")]
    [InlineData("MICIdentifier", "XNAS\n")]
    [InlineData("UUIDv4Identifier", "f47ac10b-58cc-4372-a567-0e02b2c3d479\n")]
    [InlineData("Max4AlphaNumericText", "ABC\n")]
    [InlineData("Exact3NumericText", "123\n")]
    [InlineData("ISODateTime", "2026-02-28T12:00:00\n")]
    [InlineData("ISOYear", "2026\n")]
    [InlineData("XsdNCName", "abc\n")]
    [InlineData("ExternalTrackerNotificationTypeCode", "ABC\r\n")]
    public void TextWithATrailingLineBreak_IsRefused(string type, string text)
    {
        Assert.Null(StandardsVectors.Canonical(type, text));
    }

    // DateOffsetText was written ^0|-1|-2$, which reads as three alternatives and accepted "0abc" and "x-1"
    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("-2")]
    public void DateOffsetText_AcceptsItsThreeValues(string text)
    {
        Assert.Equal(text, StandardsVectors.Canonical("DateOffsetText", text));
    }

    [Theory]
    [InlineData("0abc")]
    [InlineData("x-1")]
    [InlineData("-1x")]
    [InlineData("-3")]
    [InlineData("1")]
    [InlineData("")]
    [InlineData("0\n")]
    public void DateOffsetText_RefusesEverythingElse(string text)
    {
        Assert.Null(StandardsVectors.Canonical("DateOffsetText", text));
    }
}
