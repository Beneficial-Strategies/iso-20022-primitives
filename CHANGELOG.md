# Changelog

The versions of this package have three parts, `design.breaking.compatible`: the first changes with a redesign, the second with a change that breaks existing users, the third with every other change. Newest first. Each entry is written from the issues closed since the previous release.

## 1.1.0-alpha (2026-10-25)

* Breaking change: XsdDouble and XsdFloat refuse Infinity and nan; only INF, -INF and NaN are special values (#11).
* Breaking change: XsdDuration writes a zero duration as PT0S instead of P0D (#13).
* Breaking change: The IBAN types refuse a wrong check digit (#16).
* Breaking change: LEIIdentifier and UTIIdentifier refuse a wrong LEI check digit (#17).
* Breaking change: The ISIN types refuse a wrong check digit (#18).
* Breaking change: EANGLNIdentifier refuses a wrong check digit (#19).
* Breaking change: FedwireRoutingNumberIdentifier refuses a wrong ABA check digit (#20).
* Breaking change: The CFI types refuse a first letter that is not a category (#21).
* Breaking change: Text with a trailing line feed is refused, and DateOffsetText accepts only 0, -1 and -2 (#35).
* Breaking change: Max10DateText refuses a truncated day, and a timezone offset with minutes above 59 is refused (#39).
* Breaking change: The DTI types refuse a wrong check character (ISO 24165) (#45).
* Added: Every type that cites an external standard is tested against it, and a test keeps that list complete (#10).
* Fixed: ISOTime and ISODateTime accept 24:00:00, the end of the day (#12).
* Fixed: XsdDouble and XsdFloat write enough digits to read back as the same number on .NET Framework (#15).
* Fixed: UUIDv4Identifier reads upper-case hexadecimal and keeps lower case (#22).
* Fixed: Text lengths are counted in characters, as XML Schema counts them (#37).
* Changed: README links to the package names of the monorepo (#8).
* Known issue: NPIIdentifier checks the shape of the identifier but not its two check digits (ISO 24366); the algorithm is not yet confirmed (#26).
* Known issue: The check character of the DTI types follows a rule reconstructed from public implementations; the DTI Foundation has not yet confirmed it or the list of exceptions (#27).
* Known issue: WertpapierIdentifier accepts digits only, while German securities codes (WKN) can contain letters (#28).
* Known issue: BelgianIdentifier and DutchIdentifier accept any text; the national formats are not checked (#29).
* Known issue: The IBAN types check the check digits but not the length and structure of each country (#30).
* Known issue: The CFI types check the category letter but not the attribute letters (#31).
* Known issue: ISOCountrySubDivisionCode refuses numeric ISO 3166-2 codes such as FR-75, because the ISO 20022 pattern allows letters only (#33).
* Known issue: ISODate refuses the optional timezone that xs:date allows (#40).

## 1.0.1-alpha (2026-10-23)

* Added: `IsoDisplayNameAttribute` (the ISO name of a type, valid on an interface) and `IsoListAttribute` with `IsoListKind` (the kind of list that holds the items of a member that may repeat), so that the interfaces of the model can say what a generator needs to write the records.

## 1.0.0-alpha (2026-10-23)

* Changed: The package lives in its own repository, with its history, and is versioned and released independently of the other packages. Its code is unchanged from 0.7.0-alpha.
* Changed: Versions have three parts, `design.breaking.compatible`: the first changes with a redesign, the second with a change that breaks existing users, the third with every other change. This is not Semantic Versioning.
* Changed: The README is the package README, and its examples are projects under `examples/` that are built and run against the packed package.
* Fixed: The first example of the README did not compile for lack of `using BeneficialStrategies.Iso20022;`.

## 0.7.0-alpha

Released from the monorepo this package lived in before (Iso20022Library), together with the other packages; its history is in this repository's commits and tags.
