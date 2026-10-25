// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

namespace BeneficialStrategies.Iso20022.SimpleTypes;

public class DTI2024IdentifierTests : SimpleValueStringContractTests<DTI2024Identifier>
{
    protected override string ValidSample => "12345678K";
    protected override string InvalidSample => "012345678";
}
