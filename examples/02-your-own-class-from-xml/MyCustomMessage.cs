using BeneficialStrategies.Iso20022.Codesets;
using BeneficialStrategies.Iso20022;
using BeneficialStrategies.Iso20022.Framework;
using BeneficialStrategies.Iso20022.SimpleTypes;

namespace MyCompany.Examples;

// <snippet message>
public class MyCustomMessage : IOuterRecord
{
    public const string IsoIdentifier = "xmpl.001.001.01";
    public const string XmlTag = "Party";
    public const string DocumentNamespace = "urn:example:xmpl.001.001.01";
    public const string DocumentElementName = "Document";

    [IsoXmlTag("Name")]
    public required Max35Text Name { get; init; }                      // at most 35 characters

    [IsoXmlTag("ChargeBearer")]
    public required ChargeBearerType1Code ChargeBearer { get; init; } // DEBT, CRED, SHAR or SLEV
}
// </snippet>
