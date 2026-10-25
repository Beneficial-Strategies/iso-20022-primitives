// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace BeneficialStrategies.Iso20022.SimpleTypes;

/// <summary>
/// Global Location Number. A non-significant reference number used to identify legal entities, functional entities or physical entities according to the European Association for Numbering (EAN) numbering scheme rules.
/// </summary>
[DataContract]
[Serializable]
[IsoId("_YXlUANp-Ed-ak6NoX_4Aeg_-2105735333")]
[Description(@"Global Location Number. A non-significant reference number used to identify legal entities, functional entities or physical entities according to the European Association for Numbering (EAN) numbering scheme rules.")]

[JsonConverter(typeof(Iso20022SimpleValueJsonConverter<EANGLNIdentifier>))]
public readonly struct EANGLNIdentifier : IIsoSimpleValue<string>, IEquatable<EANGLNIdentifier>
{
    /// <summary>ISO 20022 format constraint for this type.</summary>
    public const string Pattern = @"^[0-9]{13,13}\z";

    /// <inheritdoc/>
    public string Value { get; }

    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <exception cref="Iso20022FormatException">
    /// Thrown with <see cref="Iso20022FormatViolation.PatternMismatch"/> when the value
    /// does not satisfy <see cref="Pattern"/>, or with <see cref="Iso20022FormatViolation.CheckDigitMismatch"/>
    /// when it does but its check digits are wrong (GS1, modulus 10).
    /// </exception>
    public EANGLNIdentifier(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (!Regex.IsMatch(value, Pattern))
            throw new Iso20022FormatException(typeof(EANGLNIdentifier), value, Pattern);
        if (!CheckDigits.Gs1(value))
            throw Iso20022FormatException.ForCheckDigitMismatch(typeof(EANGLNIdentifier), value, "GS1, modulus 10");
        Value = value;
    }

    /// <summary>Returns <see langword="true"/> when <paramref name="value"/> satisfies <see cref="Pattern"/> and its check digits (GS1, modulus 10).</summary>
    public static bool TryCreate(string? value, [NotNullWhen(true)] out EANGLNIdentifier result)
    {
        if (value is not null && Regex.IsMatch(value, Pattern) && CheckDigits.Gs1(value))
        { result = new(value); return true; }
        result = default;
        return false;
    }

    /// <summary>Implicitly wraps a string as a <see cref="EANGLNIdentifier"/>.</summary>
    public static implicit operator EANGLNIdentifier(string value) => new(value);
    /// <summary>Implicitly unwraps to the underlying string.</summary>
    public static implicit operator string(EANGLNIdentifier id) => id.Value;

    /// <inheritdoc/>
    public override string ToString() => Value ?? string.Empty;
    /// <inheritdoc/>
    public bool Equals(EANGLNIdentifier other) => Value == other.Value;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is EANGLNIdentifier other && Equals(other);
    /// <inheritdoc/>
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;

    /// <inheritdoc/>
    public static bool operator ==(EANGLNIdentifier a, EANGLNIdentifier b) => a.Equals(b);
    /// <inheritdoc/>
    public static bool operator !=(EANGLNIdentifier a, EANGLNIdentifier b) => !a.Equals(b);
    /// <inheritdoc/>
    public static bool operator ==(EANGLNIdentifier a, string? b) => a.Value == b;
    /// <inheritdoc/>
    public static bool operator !=(EANGLNIdentifier a, string? b) => a.Value != b;
    /// <inheritdoc/>
    public static bool operator ==(string? a, EANGLNIdentifier b) => a == b.Value;
    /// <inheritdoc/>
    public static bool operator !=(string? a, EANGLNIdentifier b) => a != b.Value;
}
