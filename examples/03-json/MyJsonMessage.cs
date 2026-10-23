using BeneficialStrategies.Iso20022;
using BeneficialStrategies.Iso20022.Framework;
using BeneficialStrategies.Iso20022.Codesets;
using BeneficialStrategies.Iso20022.SimpleTypes;

namespace MyCompany.Examples;

// The message of example 02, repeated so that this example stands alone.
public class MyJsonMessage : IOuterRecord
{
    public const string IsoIdentifier = "xmpl.001.001.01";
    public const string XmlTag = "Party";
    public const string DocumentNamespace = "urn:example:xmpl.001.001.01";
    public const string DocumentElementName = "Document";

    [IsoXmlTag("Name")]
    public required Max35Text Name { get; init; }

    [IsoXmlTag("ChargeBearer")]
    public required ChargeBearerType1Code ChargeBearer { get; init; }
}
