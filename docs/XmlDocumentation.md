# XML documentation of the public types

Moved here from the monorepo's CLAUDE.md. Every public type and member of this package carries the verbatim ISO 20022 description.

## The requirement

**Every public type and every public member must carry the verbatim ISO 20022 specification
description as its XML doc comment.** This is a hard requirement, not a style preference. The
NuGet package surfaces these descriptions directly to consumers via IntelliSense and generated
API documentation — paraphrasing, truncation, or substituting the display name destroys the
understanding the ISO spec authors encoded.

### What "verbatim" means

- **Do not paraphrase.** "Reference that uniquely identifies the message from a business
  application standpoint." is correct. "Message Identification." is not.
- **Do not infer from the name.** The display name (`[DisplayName]`) and the XML doc summary are
  separate concerns. The display name is a label; the summary is explanatory prose from the spec.
- **Do not truncate** lengthy Scope/Usage/Constraint text. If the ISO text is long, use
  `<remarks>` for the overflow — but keep it all.
- **`<seealso>` tags** are welcome where the spec cross-references other types or messages.

### Where to put the content

| ISO text | XML doc placement |
|---|---|
| Primary one-sentence description | `<summary>` |
| Scope / Usage / Constraint paragraphs (when lengthy) | `<remarks>` |
| Cross-references to related types or messages | `<seealso cref="..."/>` |

### What must be on every type

**Message record (class):**
```csharp
/// <summary>
/// {ISO primary description — the first sentence or the "Scope" intro.}
/// </summary>
/// <remarks>
/// {Full ISO Usage / Scope text, preserved verbatim, as many paragraphs as needed.}
/// </remarks>
[Description(@"Scope|{ISO pipe-delimited full text}")]
[IsoId("...")]
[DisplayName("...")]
public record MyMessageV01 : IOuterRecord { ... }
```

**Message property:**
```csharp
/// <summary>
/// {Verbatim ISO field description — NOT just the display name.}
/// </summary>
[IsoId("...")]          // present on properties in older files; may be omitted in newer style
[DisplayName("...")]
[IsoXmlTag("...")]
public required SomeType MyField { get; init; }
```

**Component record and its properties:** same rules — class gets the ISO component description,
each property gets the ISO element description.

**Codeset enum and its members:** same rules — enum gets the ISO code-set description, each
`[EnumMember]`-decorated value gets the ISO code description.

### Where to source the descriptions

Use the ISO 20022 MCP server tools:
- `universal_lookup` — returns full spec text for a named artifact (message, component, element,
  code set, code value).
- `get_data_type_members_snapshot` — bulk element descriptions for a component.
- `get_code_set_details` — full descriptions for a code set and its values.

Always fetch descriptions from the MCP server rather than guessing or deriving them from names.
If a description cannot be retrieved, leave a `// TODO: fetch ISO description via MCP` comment
rather than fabricating one.

#### Exception: primitives defined by an external (non-ISO-20022) standard

Some ISO 20022 `IdentifierSet` primitives (SEDOL, CUSIP, Valoren, WKN/Wertpapier, etc.) are
identifiers whose *own* structure — length, character set, and especially check-digit algorithms —
is defined by an external standards body (LSE, CUSIP Global Services, SIX Financial Information,
Wertpapier Mitteilungen...), not by ISO 20022 itself. ISO 20022's own text for these is often a
one-line description with no format detail at all; MCP has nothing more to give.

For these — and only these, i.e. types where the *format itself* originates outside ISO 20022, not
ordinary ISO 20022 elements — it's acceptable to research the external standard directly (web
search, published algorithm references) rather than stop at "MCP has nothing." Requirements:

- **Cite the source.** Every fact pulled from outside MCP (pattern, length, check-digit algorithm)
  gets an XML-doc `<remarks>` note and/or inline comment naming where it came from (organization,
  document, or URL) — so a future maintainer can verify or update it without redoing the research.
- **Check digits are verified and can be generated.** If the external standard defines a check
  digit, the struct validates it on construction when a full value (core + check digit) is
  supplied, and computes+appends it automatically when only the core is supplied. Expose the
  parts as separate `Core` and `CheckDigit` properties alongside the full `Value`.
- **When the external standard can't be pinned down, stay permissive and say so.** Not every one
  of these turns up a usable public spec (some are obscure legacy national schemes with no
  findable documentation). When that happens, keep the pattern permissive (e.g. "non-empty
  string") rather than guessing at structure, and leave a comment stating that external
  verification was attempted and came up empty — so it reads as "investigated, not overlooked."
  Same principle applies to genuinely uncertain details within an otherwise-known standard: default
  to the more permissive interpretation and comment the doubt for later tightening. A pattern is
  easy to tighten later; it's a breaking change to loosen one after consumers depend on it.
- **Recognize when a type isn't actually a checksummed identifier at all.** Some `IdentifierSet`
  members that look superficially similar (e.g. credit-rating symbols, hierarchical classification
  codes) aren't numbered identifiers with check digits — don't force the `Core`/`CheckDigit` shape
  onto something that isn't structured that way.

#### Exception: primitives defined by a W3C standard

Some ISO 20022 primitives are directly W3C XSD builtins or map one-to-one onto a W3C-defined
concept (`xs:anyURI`, `xs:duration`, `xs:QName`, and similar). Where the .NET BCL already ships a
type for the underlying W3C standard (e.g. something in `System.Xml`), it's fine to use that
existing type directly as the member's type instead of hand-writing a new `IIsoSimpleValue<T>`
wrapper struct — no need to reinvent parsing/validation the framework already gets right.

- This can be a **class**, not just a struct — some `System.Xml` types are reference types.
  That's fine; the ISO primitives template's "always a `readonly struct`" rule is
  specifically about hand-written `IIsoSimpleValue<T>` wrappers, not about this exception.
- A BCL type used this way won't implement `IIsoSimpleValue<T>` and won't throw
  `Iso20022FormatException` — it relies on the BCL type's own parsing/validation/serialization
  instead. Say so in a comment where it is used, so it's clear the type is intentionally outside
  the usual contract-test/serializer plumbing, not an oversight.
- Still verify the mapping is actually correct for the specific ISO 20022 primitive (e.g. does the
  BCL type's wire format match what ISO 20022 expects on the XML/JSON side?) before using it —
  this exception is about not re-implementing a solved problem, not about skipping verification.

#### Class-typed properties and hierarchical record equality

Message and component types are `record`s specifically so that `==`/`Equals` gives correct
hierarchical, structural equality without hand-written comparison code. That guarantee comes from
the compiler-synthesized `Equals`/`GetHashCode`, which calls `EqualityComparer<T>.Default` on every
property — **struct or class, it makes no difference to the mechanism.** Our `IIsoSimpleValue<T>`
structs work with this because every one of them implements `IEquatable<T>` with real value
comparison; that's *why* the struct convention is safe, not because it's a struct per se. A `class`
property (from the W3C exception above, or a hand-modeled type for embedded/raw XML content) goes
through the exact same mechanism — it only preserves the record's equality guarantee if that class
itself overrides `Equals`/`GetHashCode` for value semantics. If it doesn't (falls back to
`object.Equals`, i.e. reference identity), the record's `==` silently breaks for that property: two
records built from byte-identical input, but holding two separately-constructed instances of that
class, compare unequal — with no compiler warning, since it type-checks fine.

**Before embedding any class-typed property in a record, verify from the actual source, not just
documentation, whether it implements value equality.** Docs can be wrong: `XmlQualifiedName`'s own
Microsoft Learn page states `Equals` "returns true if the two are the same instance object"
(reference equality) — the real shipped implementation
(`dotnet/runtime`'s `XmlQualifiedName.cs`) does a proper `Name`+`Namespace` value comparison with a
reference-equality fast path first. Trusting the doc page over the source would have been wrong.
Two data points already checked, to save re-deriving them:
- `System.Xml.XmlQualifiedName` — value equality confirmed from source. Safe to embed directly.
- `System.Xml.Linq.XElement` / `XNode` / `XmlElement` — reference equality confirmed (well-documented
  .NET behavior); structural comparison is opt-in only via `XNode.DeepEquals`/`XNodeEqualityComparer`,
  and even that has known rough edges (namespace-prefix-vs-default-namespace and attribute-ordering
  differences read as "not equal" despite being semantically identical XML — see
  [Equality Semantics of LINQ to XML Trees](https://learn.microsoft.com/en-us/archive/blogs/ericwhite/equality-semantics-of-linq-to-xml-trees)).
  **Not safe to embed without one of the mitigations below.**

When a class-typed property does *not* have value equality (most likely scenario: a component that
carries embedded/raw XML content — e.g. `ExternalSchema/LaxPayload.cs`, `SignatureEnvelope.cs`,
`ATICALaxProcessing.cs`, currently unimplemented stub records with no properties yet), pick one,
in order of preference:

1. **Override `Equals`/`GetHashCode` on that record explicitly**, using `XNode.DeepEquals` (or a
   canonicalized-string comparison if robustness to `DeepEquals`'s prefix/attribute-order
   sensitivity matters) instead of letting the compiler synthesize member-wise equality for that
   property.
2. **Store canonicalized XML as a `string`** inside an `IIsoSimpleValue<string>` struct wrapper
   instead of holding a live `XElement`/`XmlElement` graph — gets correct record equality for free
   via the normal struct convention, at the cost of losing in-place LINQ-to-XML querying (a
   consumer parses the string on demand instead).
3. **Accept reference equality, but document it.** Not a disaster on its own — but say so
   explicitly in the type's `<remarks>` (e.g. "Equality for this property is reference-based; two
   instances with identical XML content are not guaranteed to compare equal") so it reads as a
   deliberate, acknowledged tradeoff instead of a silent gap someone finds the hard way, debugging
   a failing `Assert.Equal` months later.

### Historical gap

Files written during snapshot syncs prior to 2026-06 were often written without verbatim
descriptions — properties received only the display name as their `<summary>`, and class-level
`[Description(...)]` attributes were omitted. These files must be corrected as they are
encountered. The correct reference style is shown in older files such as
`MessageDefinitions/setr/RedemptionOrderV04.cs`.
