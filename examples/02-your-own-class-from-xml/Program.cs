// Example 02: a class of your own, read from and written to XML.
// Shows:  the primitive types as the members of your own message class, the reader that reports every problem it finds, and the writer.
// Needs:  only BeneficialStrategies.Iso20022.Primitives.
// Prints: the message read from valid XML, the problems found in invalid XML, and the XML written back.
using BeneficialStrategies.Iso20022.Codesets;
using BeneficialStrategies.Iso20022.Serialization;
using MyCompany.Examples;

// <snippet read>
var xml = "<Document xmlns=\"urn:example:xmpl.001.001.01\"><Party><Name>Acme Corp</Name><ChargeBearer>SHAR</ChargeBearer></Party></Document>";

DeserializationResult<MyCustomMessage> read = Iso20022XmlSerializer.TryRead<MyCustomMessage>(xml);

if (read.Succeeded)
    Console.WriteLine($"read: {read.Message!.Name} pays {read.Message.ChargeBearer}");   // read: Acme Corp pays Shared
else
    foreach (var issue in read.Issues)
        Console.WriteLine($"{issue.Code} {issue.Path}: {issue.Reason}");
// </snippet>

// <snippet read-invalid>
var bad = "<Document xmlns=\"urn:example:xmpl.001.001.01\"><Party><Name>A name that is a good deal longer than thirty-five characters</Name><ChargeBearer>FREE</ChargeBearer></Party></Document>";

foreach (var issue in Iso20022XmlSerializer.TryRead<MyCustomMessage>(bad).Issues)
    Console.WriteLine($"{issue.Code} {issue.Path}: {issue.Reason}");
// </snippet>

// <snippet write>
var mine = new MyCustomMessage { Name = "Acme Corp", ChargeBearer = ChargeBearerType1Code.Shared };
Console.WriteLine(Iso20022XmlSerializer.SerializeToString(mine));
// </snippet>
