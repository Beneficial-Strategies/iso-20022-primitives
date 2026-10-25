// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

namespace BeneficialStrategies.Iso20022.Framework;

/// <summary>
/// The length of a text as XML Schema measures it: in characters (Unicode code points), not in the
/// UTF-16 units of a .NET string. A character outside the Basic Multilingual Plane is one character
/// of the text and two units of the string.
/// </summary>
internal static class TextLength
{
    public static int CodePointCount(this string value)
    {
        int count = value.Length;
        for (int i = 0; i + 1 < value.Length; i++)
        {
            if (char.IsHighSurrogate(value[i]) && char.IsLowSurrogate(value[i + 1]))
            {
                count--;
                i++;
            }
        }
        return count;
    }
}
