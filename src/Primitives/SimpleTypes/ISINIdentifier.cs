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
[IsoId("_YYU64Np-Ed-ak6NoX_4Aeg_-1113170580")]
[Description(@"International Securities Identification Number (ISIN). A numbering system designed by the United Nation's International Organisation for Standardisation (ISO).")]
[Obsolete("Marked obsolete in the ISO 20022 specification.")]
[JsonConverter(typeof(Iso20022SimpleValueJsonConverter<ISINIdentifier>))]
public readonly struct ISINIdentifier : IIsoSimpleValue<string>, IEquatable<ISINIdentifier>
{
    /// <summary>ISO 20022 format constraint for this type.</summary>
    public const string Pattern = @"^[A-Z0-9]{12,12}$";

    /// <inheritdoc/>
    public string Value { get; }

    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <exception cref="Iso20022FormatException">
    /// Thrown with <see cref="Iso20022FormatViolation.PatternMismatch"/> when the value
    /// does not satisfy <see cref="Pattern"/>, or with <see cref="Iso20022FormatViolation.CheckDigitMismatch"/>
    /// when it does but its check digits are wrong (ISO 6166, Luhn).
    /// </exception>
    public ISINIdentifier(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (!Regex.IsMatch(value, Pattern))
            throw new Iso20022FormatException(typeof(ISINIdentifier), value, Pattern);
        if (!CheckDigits.Isin(value))
            throw Iso20022FormatException.ForCheckDigitMismatch(typeof(ISINIdentifier), value, "ISO 6166, Luhn");
        Value = value;
    }

    /// <summary>Returns <see langword="true"/> when <paramref name="value"/> satisfies <see cref="Pattern"/> and its check digits (ISO 6166, Luhn).</summary>
    public static bool TryCreate(string? value, [NotNullWhen(true)] out ISINIdentifier result)
    {
        if (value is not null && Regex.IsMatch(value, Pattern) && CheckDigits.Isin(value))
        { result = new(value); return true; }
        result = default;
        return false;
    }

    /// <summary>Implicitly wraps a string as a <see cref="ISINIdentifier"/>.</summary>
    public static implicit operator ISINIdentifier(string value) => new(value);
    /// <summary>Implicitly unwraps to the underlying string.</summary>
    public static implicit operator string(ISINIdentifier id) => id.Value;

    /// <inheritdoc/>
    public override string ToString() => Value ?? string.Empty;
    /// <inheritdoc/>
    public bool Equals(ISINIdentifier other) => Value == other.Value;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ISINIdentifier other && Equals(other);
    /// <inheritdoc/>
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;

    /// <inheritdoc/>
    public static bool operator ==(ISINIdentifier a, ISINIdentifier b) => a.Equals(b);
    /// <inheritdoc/>
    public static bool operator !=(ISINIdentifier a, ISINIdentifier b) => !a.Equals(b);
    /// <inheritdoc/>
    public static bool operator ==(ISINIdentifier a, string? b) => a.Value == b;
    /// <inheritdoc/>
    public static bool operator !=(ISINIdentifier a, string? b) => a.Value != b;
    /// <inheritdoc/>
    public static bool operator ==(string? a, ISINIdentifier b) => a == b.Value;
    /// <inheritdoc/>
    public static bool operator !=(string? a, ISINIdentifier b) => a != b.Value;
}
