// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

namespace BeneficialStrategies.Iso20022.Standards;

/// <summary>
/// XML Schema measures the length of a text in characters (Unicode code points). A character outside the
/// Basic Multilingual Plane, such as an emoji, is one character of the text and two units of a .NET string.
/// </summary>
public class TextLengthTests
{
    private const string Emoji = "\U0001F600"; // one character, two UTF-16 units

    [Theory]
    [InlineData("Exact1Text", 1)]
    [InlineData("Max35Text", 35)]
    [InlineData("Max70Text", 70)]
    [InlineData("Max140Text", 140)]
    [InlineData("Max16Text", 16)]
    public void TheLimit_IsInCharacters_NotInUtf16Units(string type, int limit)
    {
        string atLimit = string.Concat(Enumerable.Repeat(Emoji, limit));
        Assert.Equal(atLimit, StandardsVectors.Canonical(type, atLimit));
        Assert.Null(StandardsVectors.Canonical(type, atLimit + Emoji));
    }

    [Theory]
    [InlineData("Max35Text", "é")]
    [InlineData("Max35Text", "日本語")]
    [InlineData("Max35Text", "é")]   // a letter and a combining accent are two characters
    public void CharactersInsideTheBasicPlane_AreOneUnitEach(string type, string text)
    {
        Assert.Equal(text, StandardsVectors.Canonical(type, text));
        string tooLong = string.Concat(Enumerable.Repeat(text, 36));
        Assert.Null(StandardsVectors.Canonical(type, tooLong));
    }

    [Fact]
    public void ACombiningAccent_CountsAsACharacter()
    {
        // 35 letters with an accent each are 70 characters: too long for Max35Text
        string accented = string.Concat(Enumerable.Repeat("é", 18));
        Assert.Null(StandardsVectors.Canonical("Max35Text", accented));
    }
}
