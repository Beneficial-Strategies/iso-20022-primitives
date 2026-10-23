// Example 01: values that refuse what ISO 20022 does not allow.
// Shows:  building the primitive types, and what happens to a value outside the format of the standard.
// Needs:  only BeneficialStrategies.Iso20022.Primitives.
// Prints: the values built, and the message of each refusal.

// <snippet simple>
using BeneficialStrategies.Iso20022;
using BeneficialStrategies.Iso20022.Amounts;
using BeneficialStrategies.Iso20022.Codesets;
using BeneficialStrategies.Iso20022.SimpleTypes;

Max35Text name = "Acme Corp";        // converts implicitly from a string; 1 to 35 characters
Console.WriteLine(name);              // Acme Corp

try { _ = new Max35Text(new string('x', 36)); }
catch (Iso20022FormatException ex) { Console.WriteLine($"{ex.TargetType.Name}: {ex.Violation} - {ex.Message}"); }
// Max35Text: TooLong - 'Max35Text' allows at most 35 character(s) but received 36: 'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx'.

Console.WriteLine(Max35Text.TryCreate("", out _));      // False: no exception, when you would rather ask
// </snippet>

// <snippet external-code>
try { _ = new ActiveCurrencyCode("eur"); }
catch (Iso20022FormatException ex) { Console.WriteLine(ex.Message); }
// 'eur' is not a valid ActiveCurrencyCode. ISO 20022 pattern constraint: ^[A-Z]{3}$
// </snippet>

// <snippet amount>
try { _ = new ActiveCurrencyAndAmount { Currency = "EUR", Amount = -1m }; }
catch (Iso20022FormatException ex) { Console.WriteLine(ex.Message); }
// '-1' is not a valid ActiveCurrencyAndAmount. ISO 20022 pattern constraint: not negative
// </snippet>
