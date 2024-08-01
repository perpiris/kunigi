using System.Globalization;
using System.Text;

namespace Kunigi.Utilities;

public static class SlugGenerator
{
    private static readonly HashSet<char> ValidChars = [..new[] { '-', '_' }];
    private static readonly Dictionary<char, string> TransliterationMap = new()
    {
        {'α', "a"}, {'ά', "a"}, {'Α', "a"}, {'Ά', "a"},
        {'β', "b"}, {'Β', "b"},
        {'γ', "g"}, {'Γ', "g"},
        {'δ', "d"}, {'Δ', "d"},
        {'ε', "e"}, {'έ', "e"}, {'Ε', "e"}, {'Έ', "e"},
        {'ζ', "z"}, {'Ζ', "z"},
        {'η', "i"}, {'ή', "i"}, {'Η', "i"}, {'Ή', "i"},
        {'θ', "th"}, {'Θ', "th"},
        {'ι', "i"}, {'ί', "i"}, {'ϊ', "i"}, {'Ι', "i"}, {'Ί', "i"}, {'Ϊ', "i"},
        {'κ', "k"}, {'Κ', "k"},
        {'λ', "l"}, {'Λ', "l"},
        {'μ', "m"}, {'Μ', "m"},
        {'ν', "n"}, {'Ν', "n"},
        {'ξ', "x"}, {'Ξ', "x"},
        {'ο', "o"}, {'ό', "o"}, {'Ο', "o"}, {'Ό', "o"},
        {'π', "p"}, {'Π', "p"},
        {'ρ', "r"}, {'Ρ', "r"},
        {'σ', "s"}, {'ς', "s"}, {'Σ', "s"},
        {'τ', "t"}, {'Τ', "t"},
        {'υ', "y"}, {'ύ', "y"}, {'ϋ', "y"}, {'Υ', "y"}, {'Ύ', "y"}, {'Ϋ', "y"},
        {'φ', "f"}, {'Φ', "f"},
        {'χ', "ch"}, {'Χ', "ch"},
        {'ψ', "ps"}, {'Ψ', "ps"},
        {'ω', "o"}, {'ώ', "o"}, {'Ω', "o"}, {'Ώ', "o"},
        {'&', "kai"}
    };

    public static string GenerateSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        input = input.Replace(' ', '-');
        input = input.Normalize(NormalizationForm.FormD);
        input = TransliterateGreekToLatin(input);
        input = RemoveDiacritics(input).ToLowerInvariant();
        input = RemoveInvalidCharacters(input);
        input = input.Replace("--", "-", StringComparison.Ordinal);
        input = input.Trim('-');

        return input;
    }

    private static string TransliterateGreekToLatin(string input)
    {
        var result = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            if (TransliterationMap.TryGetValue(c, out var value))
                result.Append(value);
            else
                result.Append(c);
        }
        return result.ToString();
    }

    private static string RemoveDiacritics(string input)
    {
        var normalizedString = input.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

        foreach (var c in normalizedString.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
        {
            stringBuilder.Append(c);
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    private static string RemoveInvalidCharacters(string input)
    {
        return new string(input.Where(c => char.IsLetterOrDigit(c) || ValidChars.Contains(c)).ToArray());
    }
}