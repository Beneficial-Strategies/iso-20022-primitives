// Example 03: the same class as JSON.
// Shows:  the ISO naming in JSON (snake_case property names, the wire codes of the code sets), and a value that breaks its type being refused.
// Needs:  only BeneficialStrategies.Iso20022.Primitives; the class of example 02 is repeated here so this example stands alone.
// Prints: the JSON written, whether it reads back, and the refusal of an invalid code.
using BeneficialStrategies.Iso20022.Codesets;
using BeneficialStrategies.Iso20022.Serialization;
using MyCompany.Examples;

// <snippet json>
var mine = new MyJsonMessage { Name = "Acme Corp", ChargeBearer = ChargeBearerType1Code.Shared };

var json = Iso20022JsonSerializer.SerializeToString(mine);
Console.WriteLine(json);                                         // {"name":"Acme Corp","charge_bearer":"SHAR"}

var ok = Iso20022JsonSerializer.TryDeserialize<MyJsonMessage>(json, out var back, out var error);
Console.WriteLine($"read back: {ok}, {back!.Name}");             // read back: True, Acme Corp
Iso20022JsonSerializer.TryDeserialize<MyJsonMessage>("{\"name\":\"Acme Corp\",\"charge_bearer\":\"FREE\"}", out _, out error);
Console.WriteLine(error!.Message);                               // Unknown ChargeBearerType1Code value: 'FREE'
// </snippet>
