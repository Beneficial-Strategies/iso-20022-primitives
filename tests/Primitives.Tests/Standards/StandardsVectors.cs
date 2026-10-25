// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

using System.Text.Json;
using BeneficialStrategies.Iso20022.SimpleTypes;

namespace BeneficialStrategies.Iso20022.Standards;

/// <summary>
/// Reads and writes a simple type by its name, so a test class of a family is a table of text
/// and nothing else. A value is made the way a reader of a message makes it: by its text.
/// </summary>
internal static class StandardsVectors
{
    private static readonly Assembly Library = typeof(ISODateTime).Assembly;

    public static Type TypeNamed(string name) =>
        Library.GetTypes().SingleOrDefault(t => t.Name == name && t.IsPublic && t.Namespace!.StartsWith("BeneficialStrategies.Iso20022", StringComparison.Ordinal))
        ?? throw new ArgumentException($"No public type named {name}.", nameof(name));

    /// <summary>The text the type writes for <paramref name="text"/>, or <see langword="null"/> if the type refuses it.</summary>
    public static string? Canonical(string typeName, string text)
    {
        var type = TypeNamed(typeName);
        var tryCreate = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m => m.Name == "TryCreate"
                && m.GetParameters() is { Length: 2 } p
                && p[0].ParameterType == typeof(string)
                && p[1].ParameterType.GetElementType() == type);
        var args = new object?[] { text, null };
        return (bool)tryCreate.Invoke(null, args)! ? args[1]!.ToString() : null;
    }

    /// <summary>The JSON the library writes for the value made from <paramref name="text"/>.</summary>
    public static string Json(string typeName, string text)
    {
        var value = Make(typeName, text);
        return JsonSerializer.Serialize(value, value.GetType(), Iso20022JsonSerializerOptions.Default);
    }

    /// <summary>The text of the value that JSON <paramref name="json"/> is read into.</summary>
    public static string? ReadJson(string typeName, string json) =>
        JsonSerializer.Deserialize(json, TypeNamed(typeName), Iso20022JsonSerializerOptions.Default)?.ToString();

    private static object Make(string typeName, string text)
    {
        var type = TypeNamed(typeName);
        var ctor = type.GetConstructor(new[] { typeof(string) })
            ?? throw new InvalidOperationException($"{typeName} has no constructor from text.");
        return ctor.Invoke(new object[] { text });
    }
}
