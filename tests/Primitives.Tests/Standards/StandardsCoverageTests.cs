// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

using System.ComponentModel;
using System.Text.RegularExpressions;
using BeneficialStrategies.Iso20022.SimpleTypes;

namespace BeneficialStrategies.Iso20022.Standards;

/// <summary>
/// Step 4 of #10: a type whose description cites a standard other than ISO 20022 must be named in one
/// of the tests of the families in this folder, so a new type cannot arrive without its proof.
/// Types of the model that are not text, numbers or dates (the open-content wrappers) and the amounts
/// and the implied-currency amounts are listed with the reason. Enumerations are proved by the tests of the code sets.
/// </summary>
public class StandardsCoverageTests
{
    private static readonly Regex Cites = new(@"ISO (?!20022)\d+|RFC \d+|\bW3C\b|\bIEEE\b|\bUnicode\b|XML Schema", RegexOptions.CultureInvariant);

    // not text, number or date: the content is a fragment of XML, proved by the XML tests of the messages
    private static readonly HashSet<string> OpenContent = new(StringComparer.Ordinal)
    {
        "ATICALaxProcessing", "LaxPayload", "SignatureEnvelope", "SignatureEnvelopeReference", "SkipPayload", "StrictPayload",
    };

    // the amount types with an implied currency: their facets and currency rules are #13 and #22
    private static readonly HashSet<string> AmountsOfOtherIssues = new(StringComparer.Ordinal)
    {
        "ImpliedCurrencyAnd20Amount", "ImpliedCurrencyAndAmount", "RestrictedFINImpliedCurrencyAndAmount",
    };

    [Fact]
    public void EveryTypeThatCitesAStandard_IsNamedByATestOfAFamily()
    {
        string covered = string.Concat(
            typeof(StandardsCoverageTests).Assembly.GetManifestResourceNames()
                .Where(n => n.StartsWith("Standards/", StringComparison.Ordinal) && !n.EndsWith("StandardsCoverageTests.cs", StringComparison.Ordinal))
                .Select(n =>
                {
                    using var reader = new StreamReader(typeof(StandardsCoverageTests).Assembly.GetManifestResourceStream(n)!);
                    return reader.ReadToEnd();
                }));

        var missing = typeof(ISODateTime).Assembly.GetTypes()
            .Where(t => t.IsPublic && t.IsValueType && !t.IsEnum && t.Namespace is "BeneficialStrategies.Iso20022.SimpleTypes" or "BeneficialStrategies.Iso20022.Codesets" or "BeneficialStrategies.Iso20022.Amounts" or "BeneficialStrategies.Iso20022.ExternalSchema")
            .Where(t => !OpenContent.Contains(t.Name) && !AmountsOfOtherIssues.Contains(t.Name))
            .Where(t => t.GetCustomAttribute<DescriptionAttribute>() is { } d && Cites.IsMatch(d.Description))
            .Where(t => !covered.Contains($"\"{t.Name}\"", StringComparison.Ordinal))
            .Select(t => t.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();

        Assert.True(missing.Count == 0, $"Types that cite a standard and are in no family test: {string.Join(", ", missing)}");
    }
}
