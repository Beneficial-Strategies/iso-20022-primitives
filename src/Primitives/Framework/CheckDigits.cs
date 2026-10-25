// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

namespace BeneficialStrategies.Iso20022.Framework;

/// <summary>
/// The check-digit routines of the identifiers that state one. Each takes text that already
/// matches the type's pattern and says whether its check digits agree with the rest of it.
/// </summary>
internal static class CheckDigits
{
    /// <summary>ISO 7064 MOD 97-10 as used by the IBAN rearranged ("check digits last") and the LEI: letters are 10 to 35.</summary>
    public static bool Mod97(string digitsAndLetters)
    {
        int remainder = 0;
        foreach (char raw in digitsAndLetters)
        {
            char c = char.ToUpperInvariant(raw);
            if (c is >= '0' and <= '9')
                remainder = (remainder * 10 + (c - '0')) % 97;
            else if (c is >= 'A' and <= 'Z')
                remainder = (remainder * 100 + (c - 'A' + 10)) % 97;
            else
                return false;
        }
        return remainder == 1;
    }

    /// <summary>ISO 13616: the first four characters move to the end, then MOD 97-10.</summary>
    public static bool Iban(string iban) => Mod97(iban.Substring(4) + iban.Substring(0, 4));

    /// <summary>ISO 17442: MOD 97-10 over the 20 characters of the LEI.</summary>
    public static bool Lei(string lei) => Mod97(lei);

    /// <summary>ISO 23897: a UTI starts with the LEI of the entity that made it.</summary>
    public static bool UtiPrefix(string uti) => uti.Length >= 20 && Lei(uti.Substring(0, 20));

    /// <summary>ISO 6166: letters become two digits (10 to 35), then the Luhn check over the whole number.</summary>
    public static bool Isin(string isin)
    {
        if (isin.Length != 12 || isin[11] is < '0' or > '9')
            return false;
        int sum = 0;
        bool doubleIt = false;
        for (int i = isin.Length - 1; i >= 0; i--)
        {
            char c = isin[i];
            int value = c is >= '0' and <= '9' ? c - '0' : c is >= 'A' and <= 'Z' ? c - 'A' + 10 : -1;
            if (value < 0)
                return false;
            // a letter is two digits; the Luhn weights alternate per digit, from the right
            foreach (int digit in value >= 10 ? new[] { value % 10, value / 10 } : new[] { value })
            {
                int d = doubleIt ? digit * 2 : digit;
                sum += d > 9 ? d - 9 : d;
                doubleIt = !doubleIt;
            }
        }
        return sum % 10 == 0;
    }

    /// <summary>GS1: the last digit makes the weighted sum (3 and 1 from the right, starting with 3 for the digit left of the check digit) a multiple of 10.</summary>
    public static bool Gs1(string digits)
    {
        int sum = 0;
        for (int i = 0; i < digits.Length - 1; i++)
        {
            char c = digits[digits.Length - 2 - i];
            if (c is < '0' or > '9')
                return false;
            sum += (c - '0') * (i % 2 == 0 ? 3 : 1);
        }
        char last = digits[digits.Length - 1];
        return last is >= '0' and <= '9' && (sum + (last - '0')) % 10 == 0;
    }

    /// <summary>ABA routing number: weights 3, 7, 1 over the nine digits; the sum is a multiple of 10.</summary>
    public static bool Aba(string nineDigits)
    {
        if (nineDigits.Length != 9)
            return false;
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            char c = nineDigits[i];
            if (c is < '0' or > '9')
                return false;
            sum += (c - '0') * (i % 3 == 0 ? 3 : i % 3 == 1 ? 7 : 1);
        }
        return sum % 10 == 0;
    }

    /// <summary>ISO 10962: the first letter of a CFI is one of the fourteen categories.</summary>
    public static bool CfiCategory(string cfi) => cfi.Length > 0 && "CDEFHIJKLMORST".IndexOf(cfi[0]) >= 0;
}
