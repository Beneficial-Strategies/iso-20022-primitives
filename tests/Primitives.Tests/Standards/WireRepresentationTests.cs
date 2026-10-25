// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

using System.Text.Json;

namespace BeneficialStrategies.Iso20022.Standards;

/// <summary>
/// Family 1 of #10: the types that take their form from W3C XML Schema datatypes, ISO 8601 and UTC.
/// Each row is one case: the type, the text, and what the standard says about it.
/// The values are our own, written from the rules of the standard; none is copied from it.
/// </summary>
public class WireRepresentationTests
{
    // ── Accepts what the standard allows; the text written is the canonical form ──

    [Theory]
    // xs:boolean — true, false, 1 and 0; the canonical form is true or false
    [InlineData("XsdBoolean", "true", "true")]
    [InlineData("XsdBoolean", "false", "false")]
    [InlineData("XsdBoolean", "1", "true")]
    [InlineData("XsdBoolean", "0", "false")]
    // integer types — the edge of each range
    [InlineData("XsdByte", "-128", "-128")]
    [InlineData("XsdByte", "127", "127")]
    [InlineData("XsdShort", "-32768", "-32768")]
    [InlineData("XsdShort", "32767", "32767")]
    [InlineData("XsdInt", "-2147483648", "-2147483648")]
    [InlineData("XsdInt", "2147483647", "2147483647")]
    [InlineData("XsdLong", "-9223372036854775808", "-9223372036854775808")]
    [InlineData("XsdLong", "9223372036854775807", "9223372036854775807")]
    [InlineData("XsdUnsignedByte", "255", "255")]
    [InlineData("XsdUnsignedShort", "65535", "65535")]
    [InlineData("XsdUnsignedInt", "4294967295", "4294967295")]
    [InlineData("XsdUnsignedLong", "18446744073709551615", "18446744073709551615")]
    [InlineData("XsdInteger", "-1", "-1")]
    [InlineData("XsdPositiveInteger", "1", "1")]
    [InlineData("XsdNegativeInteger", "-1", "-1")]
    [InlineData("XsdNonNegativeInteger", "0", "0")]
    [InlineData("XsdNonPositiveInteger", "0", "0")]
    // a leading plus is in the lexical space; it is not written back
    [InlineData("XsdInt", "+5", "5")]
    // xs:double and xs:float — the three special values are spelled INF, -INF and NaN
    [InlineData("XsdDouble", "INF", "INF")]
    [InlineData("XsdDouble", "-INF", "-INF")]
    [InlineData("XsdDouble", "NaN", "NaN")]
    [InlineData("XsdFloat", "INF", "INF")]
    [InlineData("XsdFloat", "-INF", "-INF")]
    [InlineData("XsdFloat", "NaN", "NaN")]
    // ISO 8601 / xs:dateTime — a local time, UTC, and an offset
    [InlineData("ISODateTime", "2026-02-28T12:00:00", "2026-02-28T12:00:00")]
    [InlineData("ISODateTime", "2026-02-28T12:00:00Z", "2026-02-28T12:00:00Z")]
    [InlineData("ISODateTime", "2026-02-28T12:00:00+05:30", "2026-02-28T12:00:00+05:30")]
    [InlineData("ISODateTime", "2026-02-28T12:00:00-08:00", "2026-02-28T12:00:00-08:00")]
    [InlineData("ISODateTime", "2026-02-28T12:00:00.123Z", "2026-02-28T12:00:00.123Z")]
    [InlineData("ISODateTime", "2024-02-29T00:00:00", "2024-02-29T00:00:00")]
    [InlineData("ISODateTime", "0001-01-01T00:00:00", "0001-01-01T00:00:00")]
    [InlineData("ISODateTime", "9999-12-31T23:59:59", "9999-12-31T23:59:59")]
    // the end of the day is 24:00:00 in xs:dateTime
    [InlineData("ISODateTime", "2026-02-28T24:00:00", "2026-02-28T24:00:00", Skip = "#12: 24:00:00 is refused")]
    [InlineData("ISOTime", "12:00:00", "12:00:00")]
    [InlineData("ISOTime", "23:59:59.999", "23:59:59.999")]
    [InlineData("ISOTime", "12:00:00Z", "12:00:00Z")]
    [InlineData("ISOTime", "24:00:00", "24:00:00", Skip = "#12: 24:00:00 is refused")]
    [InlineData("ISOYear", "2026", "2026")]
    [InlineData("ISOYear", "0001", "0001")]
    [InlineData("ISOYear", "9999", "9999")]
    [InlineData("ISOYearMonth", "2026-02", "2026-02")]
    [InlineData("ISOYearMonth", "2026-12", "2026-12")]
    // the Gregorian fragments
    [InlineData("XsdGYear", "2026", "2026")]
    [InlineData("XsdGYear", "-0044", "-0044")]
    [InlineData("XsdGYear", "12345", "12345")]
    [InlineData("XsdGYearMonth", "2026-02", "2026-02")]
    [InlineData("XsdGMonth", "--02", "--02")]
    [InlineData("XsdGDay", "---31", "---31")]
    [InlineData("XsdGMonthDay", "--02-29", "--02-29")]
    [InlineData("XsdGMonthDay", "--12-31", "--12-31")]
    // xs:duration — PnYnMnDTnHnMnS, a negative sign, a fraction of a second
    [InlineData("XsdDuration", "P1Y2M3DT4H5M6S", "P1Y2M3DT4H5M6S")]
    [InlineData("XsdDuration", "PT0S", "PT0S", Skip = "#13: a zero duration is written P0D, not the canonical PT0S")]
    [InlineData("XsdDuration", "P1D", "P1D")]
    [InlineData("XsdDuration", "-P1D", "-P1D")]
    [InlineData("XsdDuration", "PT1.5S", "PT1.5S")]
    // binary
    [InlineData("XsdHexBinary", "0FB7", "0FB7")]
    [InlineData("XsdHexBinary", "", "")]
    [InlineData("XsdBase64Binary", "AQID", "AQID")]
    [InlineData("XsdBase64Binary", "AQI=", "AQI=")]
    [InlineData("XsdBase64Binary", "AQ==", "AQ==")]
    // names
    [InlineData("XsdNCName", "a", "a")]
    [InlineData("XsdNCName", "_a.b-c", "_a.b-c")]
    [InlineData("XsdName", "a:b", "a:b")]
    [InlineData("XsdNMTOKEN", "1a", "1a")]
    [InlineData("XsdNMTOKEN", "a:b", "a:b")]
    [InlineData("XsdNMTOKENS", "a b", "a b")]
    [InlineData("XsdID", "a1", "a1")]
    [InlineData("XsdIDREF", "a1", "a1")]
    [InlineData("XsdIDREFS", "a1 b2", "a1 b2")]
    // language (RFC 3066) and URI (RFC 3986)
    [InlineData("XsdLanguage", "en", "en")]
    [InlineData("XsdLanguage", "en-US", "en-US")]
    [InlineData("XsdLanguage", "de-CH-1996", "de-CH-1996")]
    [InlineData("XsdAnyURI", "https://example.org/a?b=c#d", "https://example.org/a?b=c#d")]
    [InlineData("XsdAnyURI", "relative/path", "relative/path")]
    public void Accepts_AndWritesTheCanonicalForm(string type, string text, string canonical)
    {
        Assert.Equal(canonical, StandardsVectors.Canonical(type, text));
    }

    // ── Rejects what the standard forbids ──

    [Theory]
    [InlineData("XsdBoolean", "TRUE")]
    [InlineData("XsdBoolean", "yes")]
    [InlineData("XsdBoolean", "")]
    [InlineData("XsdByte", "-129")]
    [InlineData("XsdByte", "128")]
    [InlineData("XsdByte", "1.0")]
    [InlineData("XsdShort", "-32769")]
    [InlineData("XsdShort", "32768")]
    [InlineData("XsdInt", "-2147483649")]
    [InlineData("XsdInt", "2147483648")]
    [InlineData("XsdLong", "-9223372036854775809")]
    [InlineData("XsdLong", "9223372036854775808")]
    [InlineData("XsdUnsignedByte", "256")]
    [InlineData("XsdUnsignedByte", "-1")]
    [InlineData("XsdUnsignedShort", "65536")]
    [InlineData("XsdUnsignedInt", "4294967296")]
    [InlineData("XsdUnsignedLong", "18446744073709551616")]
    [InlineData("XsdInteger", "1.5")]
    [InlineData("XsdPositiveInteger", "0")]
    [InlineData("XsdPositiveInteger", "-1")]
    [InlineData("XsdNegativeInteger", "0")]
    [InlineData("XsdNegativeInteger", "1")]
    [InlineData("XsdNonNegativeInteger", "-1")]
    [InlineData("XsdNonPositiveInteger", "1")]
    // the special values are INF, -INF and NaN, and only those spellings
    [InlineData("XsdDouble", "Infinity", Skip = "#11: Infinity is accepted")]
    [InlineData("XsdDouble", "inf")]
    [InlineData("XsdDouble", "+INF")]
    [InlineData("XsdDouble", "nan", Skip = "#11: nan is accepted")]
    [InlineData("XsdDouble", "1,5")]
    [InlineData("XsdFloat", "Infinity", Skip = "#11: Infinity is accepted")]
    [InlineData("XsdFloat", "inf")]
    [InlineData("XsdFloat", "+INF")]
    [InlineData("XsdFloat", "nan", Skip = "#11: nan is accepted")]
    // dates and times
    [InlineData("ISODateTime", "2026-02-29T00:00:00")]
    [InlineData("ISODateTime", "2026-13-01T00:00:00")]
    [InlineData("ISODateTime", "2026-00-10T00:00:00")]
    [InlineData("ISODateTime", "2026-04-31T00:00:00")]
    [InlineData("ISODateTime", "2026-02-28T25:00:00")]
    [InlineData("ISODateTime", "2026-02-28T12:60:00")]
    [InlineData("ISODateTime", "2026-02-28T12:00:60")]
    [InlineData("ISODateTime", "2026-02-28T24:00:01")]
    [InlineData("ISODateTime", "2026-02-28 12:00:00")]
    [InlineData("ISODateTime", "2026-02-28")]
    [InlineData("ISODateTime", "2026-02-28T12:00")]
    [InlineData("ISODateTime", "2026-02-28T12:00:00+14:01")]
    [InlineData("ISODateTime", "2026-02-28T12:00:00+15:00")]
    [InlineData("ISODateTime", "2026-02-28T12:00:00+0530")]
    [InlineData("ISOTime", "24:00:01")]
    [InlineData("ISOTime", "12:60:00")]
    [InlineData("ISOTime", "12:00")]
    [InlineData("ISOYear", "26")]
    [InlineData("ISOYear", "10000")]
    [InlineData("ISOYearMonth", "2026-13")]
    [InlineData("ISOYearMonth", "2026-00")]
    [InlineData("ISOYearMonth", "2026-2")]
    [InlineData("XsdGYear", "26")]
    [InlineData("XsdGYear", "+2026")]
    [InlineData("XsdGYearMonth", "2026-13")]
    [InlineData("XsdGYearMonth", "2026-00")]
    [InlineData("XsdGMonth", "--13")]
    [InlineData("XsdGMonth", "--00")]
    [InlineData("XsdGMonth", "02")]
    [InlineData("XsdGDay", "---32")]
    [InlineData("XsdGDay", "---00")]
    [InlineData("XsdGDay", "31")]
    [InlineData("XsdGMonthDay", "--02-30")]
    [InlineData("XsdGMonthDay", "--04-31")]
    [InlineData("XsdGMonthDay", "--13-01")]
    [InlineData("XsdGMonthDay", "--00-10")]
    // durations
    [InlineData("XsdDuration", "P")]
    [InlineData("XsdDuration", "PT")]
    [InlineData("XsdDuration", "P1H")]
    [InlineData("XsdDuration", "P1YT")]
    [InlineData("XsdDuration", "P1D2Y")]
    [InlineData("XsdDuration", "1Y")]
    [InlineData("XsdDuration", "P-1Y")]
    // binary: whole bytes only, and base64 is padded to a multiple of four
    [InlineData("XsdHexBinary", "0FB")]
    [InlineData("XsdHexBinary", "0G")]
    [InlineData("XsdBase64Binary", "AQ")]
    [InlineData("XsdBase64Binary", "AQI")]
    [InlineData("XsdBase64Binary", "A===")]
    [InlineData("XsdBase64Binary", "AQ!=")]
    // names
    [InlineData("XsdNCName", "1a")]
    [InlineData("XsdNCName", "a:b")]
    [InlineData("XsdNCName", "")]
    [InlineData("XsdNCName", " a")]
    [InlineData("XsdName", "1a")]
    [InlineData("XsdName", "")]
    [InlineData("XsdNMTOKEN", "a b")]
    [InlineData("XsdNMTOKEN", "")]
    [InlineData("XsdNMTOKENS", "")]
    [InlineData("XsdID", "1a")]
    [InlineData("XsdID", "a:b")]
    [InlineData("XsdIDREF", "1a")]
    [InlineData("XsdIDREFS", "")]
    // RFC 3066: the first subtag is one to eight letters; subtags are separated by a hyphen
    [InlineData("XsdLanguage", "")]
    [InlineData("XsdLanguage", "en_US")]
    [InlineData("XsdLanguage", "abcdefghi")]
    [InlineData("XsdLanguage", "e1")]
    public void Rejects_WhatTheStandardForbids(string type, string text)
    {
        Assert.Null(StandardsVectors.Canonical(type, text));
    }

    // ── Writes the standard's representation ──

    [Theory]
    [InlineData("XsdBoolean", "1", "true")]
    [InlineData("XsdBoolean", "0", "false")]
    [InlineData("XsdInt", "+5", "5")]
    [InlineData("XsdDouble", "INF", "INF")]
    [InlineData("XsdDouble", "NaN", "NaN")]
    [InlineData("XsdDouble", "-INF", "-INF")]
    [InlineData("XsdFloat", "INF", "INF")]
    [InlineData("ISODateTime", "2026-02-28T12:00:00Z", "2026-02-28T12:00:00Z")]
    [InlineData("ISODateTime", "2026-02-28T12:00:00+05:30", "2026-02-28T12:00:00+05:30")]
    [InlineData("ISODateTime", "2026-02-28T12:00:00", "2026-02-28T12:00:00")]
    [InlineData("ISOTime", "12:00:00Z", "12:00:00Z")]
    [InlineData("XsdDuration", "P1Y2M3DT4H5M6S", "P1Y2M3DT4H5M6S")]
    [InlineData("XsdGYear", "-0044", "-0044")]
    [InlineData("XsdGMonthDay", "--02-29", "--02-29")]
    [InlineData("XsdHexBinary", "0FB7", "0FB7")]
    [InlineData("XsdBase64Binary", "AQI=", "AQI=")]
    public void Json_WritesTheStandardsForm_AndReadsItBack(string type, string text, string written)
    {
        var json = StandardsVectors.Json(type, text);
        // System.Text.Json escapes "+" as \u002B, which is the same JSON string; compare the string
        Assert.Equal(written, JsonDocument.Parse(json).RootElement.GetString());
        Assert.Equal(written, StandardsVectors.ReadJson(type, json));
    }

    // ── Round trip: the text written is read back to the same text ──

    [Theory]
    [InlineData("XsdDouble", "1.5")]
    [InlineData("XsdDouble", "-0.000001")]
    [InlineData("XsdDouble", "1E+20")]
    [InlineData("XsdDouble", "1.7976931348623157E+308")]
    [InlineData("XsdFloat", "1.5")]
    [InlineData("XsdFloat", "3.4028235E+38")]
    [InlineData("ISODateTime", "2026-02-28T12:00:00.123456")]
    [InlineData("XsdDuration", "PT1.5S")]
    public void RoundTrips_ToTheSameText(string type, string text)
    {
        var written = StandardsVectors.Canonical(type, text);
        Assert.NotNull(written);
        Assert.Equal(written, StandardsVectors.Canonical(type, written!));
    }
}
