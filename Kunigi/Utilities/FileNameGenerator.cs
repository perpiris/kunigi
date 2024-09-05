using System.Security.Cryptography;
using System.Text;

namespace Kunigi.Utilities;

public class FileNameGenerator
{
    public static string GetUniqueFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var uniqueHash = GenerateUniqueFileHash(fileName);
        return $"{uniqueHash}{extension}";
    }

    private static string GenerateUniqueFileHash(string fileName)
    {
        var input = $"{fileName}_{DateTime.UtcNow.Ticks}";
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));

        var builder = new StringBuilder();
        foreach (var t in hashBytes)
        {
            builder.Append(t.ToString("x2"));
        }

        return builder.ToString()[..16];
    }
}