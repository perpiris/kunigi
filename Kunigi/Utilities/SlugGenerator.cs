using System.Text;

namespace Kunigi.Utilities;

public static class SlugGenerator
{
    public static string GenerateSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        
        input = TransliterateGreekToLatin(input);
        input = input.ToLowerInvariant();
        input = RemoveInvalidCharacters(input);
        input = input.Replace(' ', '-');
        input = input.Replace("--", "-");
        input = input.Trim('-');

        return input;
    }

    private static string TransliterateGreekToLatin(string input)
    {
        var transliterationMap = new Dictionary<char, string>
        {
            {'α', "a"},
            {'β', "b"},
            {'γ', "g"},
            {'δ', "d"},
            {'ε', "e"},
            {'ζ', "z"},
            {'η', "i"},
            {'θ', "th"},
            {'ι', "i"},
            {'κ', "k"},
            {'λ', "l"},
            {'μ', "m"},
            {'ν', "n"},
            {'ξ', "x"},
            {'ο', "o"},
            {'π', "p"},
            {'ρ', "r"},
            {'σ', "s"},
            {'τ', "t"},
            {'υ', "y"},
            {'φ', "f"},
            {'χ', "x"},
            {'ψ', "ps"},
            {'ω', "o"}
        };
        
        var result = new StringBuilder();
        foreach (var c in input)
        {
            if (transliterationMap.TryGetValue(c, out var value))
                result.Append(value);
            else
                result.Append(c);
        }

        return result.ToString();
    }

    private static string RemoveInvalidCharacters(string input)
    {
        var validChars = new StringBuilder();
        foreach (var c in input.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
        {
            validChars.Append(c);
        }
        return validChars.ToString();
    }
}