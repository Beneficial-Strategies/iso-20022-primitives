# Design pattern of the primitive types

Moved here from the monorepo's CLAUDE.md, where it described this package before the package had its own repository.

## The pattern, and why it is not negotiable

**An empty `enum` is never an acceptable type for a field in this library.** If a user cannot
assign a value to a `required` property, the library is broken — full stop.

All ISO 20022 primitive types that carry constraints (pattern, length, range, character set) are
represented as `readonly struct` wrappers implementing `IIsoSimpleValue<T>`. This covers:

- External code sets (open-value registries like `ExternalBankTransactionDomainCode`)
- Spec-fixed string types (`BICIdentifier`, `IBANIdentifier`, `Max35Text`, etc.)
- Decimal types with range/precision constraints (`IsoDecimal`, etc.)
- Binary types for cryptographic material (`SignatureValue`, `DigestValue`, etc.)

#### Interface Hierarchy (`Framework/`)

```
IIsoSimpleValue<T>          — base: any ISO primitive wrapping a CLR value of type T
  IIsoExternalCode          — semantic marker: value list is an external ISO registry
```

`IIsoSimpleValue<T>` is the serialization hook. Both `Iso20022XmlSerializer` and
`Iso20022ExternalCodeJsonConverter<T>` detect this interface to treat the type as a scalar.
Implementing the interface is what registers correct serialization behavior — no per-type
XML attribute is needed.

#### Struct Template

Every primitive struct follows this exact shape. Replace `MyCode`, `string`, `Pattern`,
and the `[IsoId]`/`[Description]` values:

```csharp
[DataContract]
[Serializable]
[IsoId("...")]
[Description(@"...")]
[JsonConverter(typeof(Iso20022ExternalCodeJsonConverter<MyCode>))]   // string types
public readonly struct MyCode : IIsoExternalCode, IEquatable<MyCode>
{
    /// <summary>ISO 20022 format constraint for this type.</summary>
    public const string Pattern = @"^[A-Z0-9]{1,4}$";

    /// <inheritdoc/>
    public string Value { get; }

    /// <exception cref="Iso20022FormatException">Value does not satisfy <see cref="Pattern"/>.</exception>
    public MyCode(string value)
    {
        if (!Regex.IsMatch(value, Pattern))
            throw new Iso20022FormatException(typeof(MyCode), value, Pattern);
        Value = value;
    }

    public static bool TryCreate(string value, [NotNullWhen(true)] out MyCode result)
    {
        if (Regex.IsMatch(value, Pattern)) { result = new(value); return true; }
        result = default;
        return false;
    }

    public static implicit operator MyCode(string value) => new(value);
    public static implicit operator string(MyCode code) => code.Value;

    public override string ToString() => Value ?? string.Empty;

    public bool Equals(MyCode other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is MyCode other && Equals(other);
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;

    public static bool operator ==(MyCode a, MyCode b)    => a.Equals(b);
    public static bool operator !=(MyCode a, MyCode b)    => !a.Equals(b);
    public static bool operator ==(MyCode a, string? b)   => a.Value == b;
    public static bool operator !=(MyCode a, string? b)   => a.Value != b;
    public static bool operator ==(string? a, MyCode b)   => a == b.Value;
    public static bool operator !=(string? a, MyCode b)   => a != b.Value;
}
```

**For `IIsoSimpleValue<decimal>` types:** the constructor receives a `decimal` (not a string).
The XML serializer parses text → `decimal.Parse()` → passes to the constructor. The constructor
then applies ISO-specific constraints (min/max, scale).

**For `IIsoSimpleValue<byte[]>` types** (digital signatures, digests, certificates): the
constructor receives a `byte[]`. The XML serializer base64-decodes → passes to constructor.
These types may have additional behavioral contracts (e.g. minimum length, OID validation)
tested separately.

#### Hybrid Pattern: External Code Set With Known Members

Some `IIsoExternalCode` types are classified by the ISO 20022 data dictionary as an **external
code set** ("codes maintained outside ISO 20022" — the registry can add new codes without a new
ISO 20022 schema version) **and yet the MCP server's `get_code_set_details` tool still returns a
concrete, named member table for them** (fetched from a secondary published external-registry
file, not just the base repository). `ExternalAuthenticationMethod1Code` is the first confirmed
example: 68 named codes with full definitions, despite being formally external.

**Always verify this per type — don't assume.** Call `get_code_set_details` and check whether the
response actually contains member rows. Many external code sets genuinely have no data available
via MCP; those stay a plain open struct per the template above, nothing more. Only apply this
hybrid when MCP demonstrably returns members.

**When it does, do not convert the type to a closed `enum`.** The struct must stay exactly as
open as the base template — same `(string)` constructor validating only length/pattern, same
`TryCreate`, same implicit operators — because the "external code set" classification means a
registry addition next month is a valid ISO 20022 value with zero schema-version signal. A closed
`enum` would reject that value outright: a real interoperability bug, not a theoretical one.

**Instead, add the known members as `public static readonly` named instances alongside the open
struct** — giving IntelliSense/compile-time-friendly discoverability for the values known today,
without narrowing what the constructor accepts:

```csharp
public readonly struct MyExternalCode : IIsoExternalCode, IEquatable<MyExternalCode>
{
    // ... Pattern, Value, constructor, TryCreate, operators — unchanged from the base template ...

    // ── Known values (per ISO 20022 external registry snapshot, via MCP get_code_set_details) ──
    // Convenience only — the constructor above still accepts any value satisfying Pattern,
    // including future registry additions not listed here.

    /// <summary>Serial Number of the acceptor's certificate.</summary>
    [IsoId("_w_NQIbJSEe-rYMhHpAEI4A")]
    [Description(@"Serial Number of the acceptor's certificate.")]
    public static readonly MyExternalCode AcceptorCertificateSerialNumber = new("ACSN");

    // ... one such field per member MCP returned ...
}
```

Preserve, per member, everything the equivalent `enum` member would have carried: the field name
(matches the original enum member identifier — least astonishment, no reason to rename), the
verbatim `<summary>`/`[Description]` text (same non-negotiable rule as any other public member —
see "XML Documentation" above), and `[IsoId]` set to that specific **code's own** id from the MCP
table (not the codeset's id, which belongs on the type-level `[IsoId]` instead).

**Retrofit sweep of pre-existing conversions — done 2026-08-13, re-confirmed 2026-08-16, both came
back empty.** All 35 `IIsoExternalCode` structs converted before this pattern was identified (the
original Batch 1 primitives pass, plus a handful of other Bucket B types) were individually
re-checked against `get_code_set_details` on two separate occasions, 3 days apart. Both times every
one returned zero member rows — none had unused data to retrofit, so all 35 are confirmed already
in their correct final form (plain open struct, no known-value constants; nothing missing, nothing
still an enum). Full list: `BusinessMessagePriorityCode`, `CountryCode`, `CountrySubDivisionCode`,
`EuroCurrencyCode`, `UnicodeChartsCode`, `ExternalClearingSystemMemberCode_Obsolete`, and 29
`External*Code`/`External*1Code` pairs (bank-transaction domain/family/sub-family, card-transaction
category, cheque-agent instruction, communication format, credit-line type, entity size/type,
financial-institution identification, financial-instrument product type, industry-sector
classification, mandate-setup reason, payment-scenario). If the MCP snapshot is ever refreshed with
new external-registry data, this sweep is worth re-running — `get_code_set_details` per type, same
as before.

#### Serialization Contract

| Layer | String types | Decimal types | Binary types |
|---|---|---|---|
| XML | `IIsoSimpleValue<string>` detected → text content | `IIsoSimpleValue<decimal>` → `decimal.Parse()` → ctor | `IIsoSimpleValue<byte[]>` → Base64 decode → ctor |
| JSON | `[JsonConverter(typeof(Iso20022ExternalCodeJsonConverter<T>))]` on class | Same converter, generic over T | Same converter |
| Error | `Iso20022FormatException` from ctor → wrapped as `JsonException` / `InvalidOperationException` | ← same | ← same |

#### Equality never exceeds the wire text

A primitive's equality (`Equals`, `==`, `GetHashCode`) must not be finer than what its serialized form preserves: two values
are equal if and only if they are written as the same text, or the type is coarser than its text on purpose (a `decimal`
reads `1.0` and `1.00` as one number). A type must not keep state that the text drops, such as a timezone for a format that has no
timezone, because two values would be unequal yet write identically. `ISOTime` and `ISODateTime` carry a timezone because
their formats include one; `ISODate`'s format is `YYYY-MM-DD`, so it has none and a text with a timezone is refused.
`SimpleValueNativeScalarContractTests` states the rule as a test.

#### Contract Test Registration

Every `IIsoSimpleValue<T>` type must have an entry in the contract test data source in
`Codesets/SimpleValueContractTests.cs`. The test verifies: valid construction, invalid
construction throws `Iso20022FormatException`, `TryCreate` both paths, struct/struct equality,
struct/value equality, `ToString`, JSON round-trip, XML round-trip via the serializer.

Adding a new primitive = one new entry in the data dictionary. A meta-test fails if any
`IIsoSimpleValue<T>` type in the assembly is missing from the dictionary.

Special types with additional behavioral requirements (binary, signatures) still participate
in the base contract test and add their own test methods alongside it.

#### Before writing or modifying any primitive type, verify

1. Is it an `enum` with zero members? → Must be converted to `readonly struct`.
2. Does the struct implement `IIsoSimpleValue<T>` (or `IIsoExternalCode`)? → Required.
3. Does the constructor validate and throw `Iso20022FormatException` on failure? → Required.
4. Are all six equality operators present (struct/struct and struct/T)? → Required.
5. Is there an entry in the contract test data source? → Required.
