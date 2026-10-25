// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace BeneficialStrategies.Iso20022.SimpleTypes;

/// <summary>
/// ISO 20022 ISOMonth — a calendar month, written <c>MM</c>.
/// </summary>
/// <remarks>
/// Per the ISO 20022 data dictionary: "Month represented by MM (ISO 8601)." The value is the month
/// number 1 to 12 and is written with two digits (<c>"03"</c>, not <c>"3"</c>), so the text is the
/// same as the one that was read. Implements <see cref="IIsoCompositeSimpleValue"/> so that the
/// serializer uses this type's own <see cref="ToString"/> rather than the unpadded <see cref="int"/>
/// formatter.
/// </remarks>
[DataContract]
[Serializable]
[IsoId("_-INONIm5Eeipw6hHPgB4Sw")]
[Description(@"Month represented by MM (ISO 8601).")]
[JsonConverter(typeof(Iso20022NumericValueJsonConverter<ISOMonth, int>))]
public readonly struct ISOMonth : IIsoSimpleValue<int>, IIsoCompositeSimpleValue, IEquatable<ISOMonth>
{
    /// <summary>Pattern for the lexical form: exactly 4 digits.</summary>
    public const string Pattern = @"^(0[1-9]|1[0-2])\z";

    /// <inheritdoc/>
    public int Value { get; }

    /// <summary>Initializes from the native year value.</summary>
    /// <exception cref="Iso20022FormatException">Thrown when <paramref name="value"/> is outside 0-9999 (what a 4-digit wire form can represent).</exception>
    public ISOMonth(int value)
    {
        if (value is < 1 or > 12)
            throw new Iso20022FormatException(typeof(ISOMonth), value.ToString(CultureInfo.InvariantCulture), "1 to 12 (a 2-digit MM wire form)");
        Value = value;
    }

    /// <summary>Initializes by parsing the ISO 20022 ISOMonth lexical form: exactly 4 digits.</summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <exception cref="Iso20022FormatException">Thrown when the string is not a valid 4-digit year.</exception>
    public ISOMonth(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (!Regex.IsMatch(value, Pattern))
            throw new Iso20022FormatException(typeof(ISOMonth), value, Pattern);
        Value = int.Parse(value, CultureInfo.InvariantCulture);
    }

    /// <summary>Returns <see langword="true"/> when <paramref name="value"/> is within 0-9999.</summary>
    public static bool TryCreate(int value, [NotNullWhen(true)] out ISOMonth result)
    {
        try
        { result = new(value); return true; }
        catch (Iso20022FormatException) { result = default; return false; }
    }

    /// <summary>Returns <see langword="true"/> when <paramref name="value"/> is a valid ISOMonth lexical form.</summary>
    public static bool TryCreate(string? value, [NotNullWhen(true)] out ISOMonth result)
    {
        if (value is not null && Regex.IsMatch(value, Pattern))
        { result = new(int.Parse(value, CultureInfo.InvariantCulture)); return true; }
        result = default;
        return false;
    }

    /// <summary>Implicitly wraps a native <see cref="int"/> as an <see cref="ISOMonth"/>.</summary>
    public static implicit operator ISOMonth(int value) => new(value);
    /// <summary>Implicitly unwraps to the native <see cref="int"/> value.</summary>
    public static implicit operator int(ISOMonth v) => v.Value;
    /// <summary>Implicitly wraps a wire string as an <see cref="ISOMonth"/>.</summary>
    public static implicit operator ISOMonth(string value) => new(value);
    /// <summary>Implicitly unwraps to the wire string representation.</summary>
    public static implicit operator string(ISOMonth v) => v.ToString();

    /// <inheritdoc/>
    public override string ToString() => Value.ToString("D2", CultureInfo.InvariantCulture);
    /// <inheritdoc/>
    public bool Equals(ISOMonth other) => Value == other.Value;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ISOMonth other && Equals(other);
    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc/>
    public static bool operator ==(ISOMonth a, ISOMonth b) => a.Value == b.Value;
    /// <inheritdoc/>
    public static bool operator !=(ISOMonth a, ISOMonth b) => a.Value != b.Value;
    /// <inheritdoc/>
    public static bool operator ==(ISOMonth a, int b) => a.Value == b;
    /// <inheritdoc/>
    public static bool operator !=(ISOMonth a, int b) => a.Value != b;
    /// <inheritdoc/>
    public static bool operator ==(int a, ISOMonth b) => a == b.Value;
    /// <inheritdoc/>
    public static bool operator !=(int a, ISOMonth b) => a != b.Value;
    /// <inheritdoc/>
    public static bool operator ==(ISOMonth a, string? b) => a.ToString() == b;
    /// <inheritdoc/>
    public static bool operator !=(ISOMonth a, string? b) => a.ToString() != b;
    /// <inheritdoc/>
    public static bool operator ==(string? a, ISOMonth b) => a == b.ToString();
    /// <inheritdoc/>
    public static bool operator !=(string? a, ISOMonth b) => a != b.ToString();
}
