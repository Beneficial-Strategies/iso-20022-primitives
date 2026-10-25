// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace BeneficialStrategies.Iso20022.SimpleTypes;

/// <summary>
/// International Securities Identification Number (ISIN). A numbering system designed by the United Nation's International Organisation for Standardisation (ISO).
/// </summary>
[DataContract]
[Serializable]
[IsoId("_BA0fIWecEeu_A-YoSMMq_w")]
[Description(@"International Securities Identification Number (ISIN). A numbering system designed by the United Nation's International Organisation for Standardisation (ISO).")]
[Obsolete("Marked obsolete in the ISO 20022 specification.")]
[JsonConverter(typeof(Iso20022SimpleValueJsonConverter<ISIN2021Identifier>))]
public readonly struct ISIN2021Identifier : IIsoSimpleValue<string>, IEquatable<ISIN2021Identifier>
{
    /// <summary>ISO 20022 format constraint for this type.</summary>
    public const string Pattern = @"^[A-Z]{2,2}[A-Z0-9]{9,9}[0-9]{1,1}$";

    /// <inheritdoc/>
    public string Value { get; }

    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <exception cref="Iso20022FormatException">
    /// Thrown with <see cref="Iso20022FormatViolation.PatternMismatch"/> when the value
    /// does not satisfy <see cref="Pattern"/>, or with <see cref="Iso20022FormatViolation.CheckDigitMismatch"/>
    /// when it does but its check digits are wrong (ISO 6166, Luhn).
    /// </exception>
    public ISIN2021Identifier(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (!Regex.IsMatch(value, Pattern))
            throw new Iso20022FormatException(typeof(ISIN2021Identifier), value, Pattern);
        if (!CheckDigits.Isin(value))
            throw Iso20022FormatException.ForCheckDigitMismatch(typeof(ISIN2021Identifier), value, "ISO 6166, Luhn");
        Value = value;
    }

    /// <summary>Returns <see langword="true"/> when <paramref name="value"/> satisfies <see cref="Pattern"/> and its check digits (ISO 6166, Luhn).</summary>
    public static bool TryCreate(string? value, [NotNullWhen(true)] out ISIN2021Identifier result)
    {
        if (value is not null && Regex.IsMatch(value, Pattern) && CheckDigits.Isin(value))
        { result = new(value); return true; }
        result = default;
        return false;
    }

    /// <summary>Implicitly wraps a string as a <see cref="ISIN2021Identifier"/>.</summary>
    public static implicit operator ISIN2021Identifier(string value) => new(value);
    /// <summary>Implicitly unwraps to the underlying string.</summary>
    public static implicit operator string(ISIN2021Identifier id) => id.Value;

    /// <inheritdoc/>
    public override string ToString() => Value ?? string.Empty;
    /// <inheritdoc/>
    public bool Equals(ISIN2021Identifier other) => Value == other.Value;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ISIN2021Identifier other && Equals(other);
    /// <inheritdoc/>
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;

    /// <inheritdoc/>
    public static bool operator ==(ISIN2021Identifier a, ISIN2021Identifier b) => a.Equals(b);
    /// <inheritdoc/>
    public static bool operator !=(ISIN2021Identifier a, ISIN2021Identifier b) => !a.Equals(b);
    /// <inheritdoc/>
    public static bool operator ==(ISIN2021Identifier a, string? b) => a.Value == b;
    /// <inheritdoc/>
    public static bool operator !=(ISIN2021Identifier a, string? b) => a.Value != b;
    /// <inheritdoc/>
    public static bool operator ==(string? a, ISIN2021Identifier b) => a == b.Value;
    /// <inheritdoc/>
    public static bool operator !=(string? a, ISIN2021Identifier b) => a != b.Value;
}
