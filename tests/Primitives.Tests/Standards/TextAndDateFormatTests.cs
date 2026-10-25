// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

namespace BeneficialStrategies.Iso20022.Standards;

/// <summary>
/// Families 4 and 5 of #10: text under the SWIFT FIN character sets, hexadecimal text, and the date
/// formats that ISO 20022 names without a standard of their own (Julian date, date offset, short dates).
/// The patterns are the ones of the ISO 20022 repository; each case is one value of the rule.
/// </summary>
public class TextAndDateFormatTests
{
    [Theory]
    // FIN: no slash at the beginning or the end, and no double slash
    [InlineData("RestrictedFINMax8Text", "abc")]
    [InlineData("RestrictedFINMax8Text", "a/b")]
    [InlineData("RestrictedFINMax8Text", "ab/cd/ef")]
    [InlineData("RestrictedFINMax8Text", "12345678")]
    [InlineData("RestrictedFINMax16Text", "REF/2026/0001")]
    // character set Z: letters, digits, space, line breaks and ! " % & * ; < > . , ( ) / = ' + : ? @ # { - _
    [InlineData("RestrictedFINZMax256Text", "Abc xyz 0123456789")]
    [InlineData("RestrictedFINZMax256Text", "! \" % & * ; < > . , ( ) / = ' + : ? @ # { - _")]
    [InlineData("RestrictedFINZMax256Text", "line one\r\nline two")]
    // hexadecimal text
    [InlineData("Exact1HexBinaryText", "0F")]
    [InlineData("Exact1HexBinaryText", "A9")]
    [InlineData("Max10KHexBinaryText", "0FB7")]
    [InlineData("Max10KHexBinaryText", "00")]
    // short dates: YY-MM, YYYY-MM, YYYY-MM-DD
    [InlineData("Max10DateText", "26-02")]
    [InlineData("Max10DateText", "2026-02")]
    [InlineData("Max10DateText", "2026-02-28")]
    // years
    [InlineData("ISORestrictedYear", "1900")]
    [InlineData("ISORestrictedYear", "2026")]
    [InlineData("ISORestrictedYear", "2099")]
    // Julian date: one digit for the year and the day of the year 001 to 366
    [InlineData("JulianDate", "6001")]
    [InlineData("JulianDate", "0365")]
    [InlineData("JulianDate", "9366")]
    // date offset: today, one or two business days back
    [InlineData("DateOffsetText", "0")]
    [InlineData("DateOffsetText", "-1")]
    [InlineData("DateOffsetText", "-2")]
    // XML Schema strings without line breaks and tabs
    [InlineData("XsdNormalizedString", "a b  c")]
    [InlineData("XsdToken", "a b c")]
    public void Accepts_TheFormatTheRepositoryGives(string type, string text)
    {
        Assert.Equal(text, StandardsVectors.Canonical(type, text));
    }

    [Theory]
    [InlineData("RestrictedFINMax8Text", "/abc")]
    [InlineData("RestrictedFINMax8Text", "abc/")]
    [InlineData("RestrictedFINMax8Text", "a//b")]
    [InlineData("RestrictedFINMax8Text", "123456789")]
    [InlineData("RestrictedFINMax8Text", "")]
    [InlineData("RestrictedFINZMax256Text", "a`b")]
    [InlineData("RestrictedFINZMax256Text", "a~b")]
    [InlineData("RestrictedFINZMax256Text", "a|b")]
    [InlineData("RestrictedFINZMax256Text", "a\\b")]
    [InlineData("RestrictedFINZMax256Text", "a$b")]
    [InlineData("RestrictedFINZMax256Text", "a[b")]
    [InlineData("RestrictedFINZMax256Text", "a}b")]
    [InlineData("RestrictedFINZMax256Text", "a^b")]
    [InlineData("RestrictedFINZMax256Text", "café")]
    [InlineData("RestrictedFINZMax256Text", "")]
    [InlineData("Exact1HexBinaryText", "F")]
    [InlineData("Exact1HexBinaryText", "0F0F")]
    [InlineData("Exact1HexBinaryText", "0f")]
    [InlineData("Exact1HexBinaryText", "GG")]
    [InlineData("Max10KHexBinaryText", "0FB")]
    [InlineData("Max10KHexBinaryText", "")]
    [InlineData("Max10KHexBinaryText", "0fb7")]
    [InlineData("Max10DateText", "2026-2")]
    [InlineData("Max10DateText", "26-2")]
    [InlineData("Max10DateText", "2026-02-2")]
    [InlineData("Max10DateText", "2026/02/28")]
    [InlineData("Max10DateText", "20260228")]
    [InlineData("Max10DateText", "2026-02-28\n")]
    [InlineData("ISORestrictedYear", "1899")]
    [InlineData("ISORestrictedYear", "2100")]
    [InlineData("ISORestrictedYear", "99999")]
    [InlineData("ISORestrictedYear", "abcd")]
    [InlineData("JulianDate", "6000")]
    [InlineData("JulianDate", "6367")]
    [InlineData("JulianDate", "0000")]
    [InlineData("JulianDate", "601")]
    [InlineData("JulianDate", "60011")]
    [InlineData("JulianDate", "A001")]
    [InlineData("DateOffsetText", "1")]
    [InlineData("DateOffsetText", "-3")]
    [InlineData("DateOffsetText", "0abc")]
    [InlineData("XsdNormalizedString", "a\tb")]
    [InlineData("XsdNormalizedString", "a\nb")]
    [InlineData("XsdToken", "a\tb")]
    [InlineData("XsdToken", "a\rb")]
    public void Rejects_WhatTheFormatForbids(string type, string text)
    {
        Assert.Null(StandardsVectors.Canonical(type, text));
    }

    // ── Timezone designators (xs:time, xs:dateTime): Z or an offset up to 14:00 ──

    [Theory]
    [InlineData("ISOTime", "12:00:00Z", "12:00:00Z")]
    [InlineData("ISOTime", "12:00:00+00:00", "12:00:00Z")]
    [InlineData("ISOTime", "12:00:00-00:00", "12:00:00Z")]
    [InlineData("ISOTime", "12:00:00+14:00", "12:00:00+14:00")]
    [InlineData("ISOTime", "12:00:00-14:00", "12:00:00-14:00")]
    [InlineData("ISOTime", "12:00:00+05:30", "12:00:00+05:30")]
    [InlineData("ISOTime", "12:00:00-09:30", "12:00:00-09:30")]
    [InlineData("XsdGYear", "2026+05:30", "2026+05:30")]
    [InlineData("XsdGYear", "2026Z", "2026Z")]
    public void Timezone_AcceptsZAndOffsetsUpToFourteenHours(string type, string text, string canonical)
    {
        Assert.Equal(canonical, StandardsVectors.Canonical(type, text));
    }

    [Theory]
    [InlineData("ISOTime", "12:00:00+14:01")]
    [InlineData("ISOTime", "12:00:00-14:01")]
    [InlineData("ISOTime", "12:00:00+15:00")]
    [InlineData("ISOTime", "12:00:00+05:60")]
    [InlineData("ISOTime", "12:00:00+5:30")]
    [InlineData("ISOTime", "12:00:00+0530")]
    [InlineData("ISOTime", "12:00:00+05")]
    [InlineData("ISOTime", "12:00:00z")]
    [InlineData("ISOTime", "12:00:00 Z")]
    [InlineData("ISOTime", "12:00:00UTC")]
    [InlineData("XsdGYear", "2026+14:01")]
    public void Timezone_RefusesWhatXmlSchemaForbids(string type, string text)
    {
        Assert.Null(StandardsVectors.Canonical(type, text));
    }
}
