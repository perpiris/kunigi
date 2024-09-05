using System.Security.Cryptography;
using System.Text;
using Kunigi.Data;
using Kunigi.Exceptions;

namespace Kunigi.Services.Implementation;

public class MediaService : IMediaService
{
    private readonly string _mediaPath;
    private readonly DataContext _context;

    public MediaService(IConfiguration configuration, DataContext context)
    {
        _mediaPath = configuration["ImageStoragePath"];
        _context = context;
    }

    public string CreateFolder(string path)
    {
        var fullPath = NormalizeAndCombinePaths(_mediaPath, path);
        Directory.CreateDirectory(fullPath);

        return fullPath;
    }

    public async Task<string> SaveMediaFile(IFormFile file, string path, bool isProfileImage)
    {
        var fullPath = NormalizeAndCombinePaths(_mediaPath, path);
        var fileName = GetUniqueFileName(file.FileName, isProfileImage);
        var filePath = Path.Combine(fullPath, fileName);

        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await file.CopyToAsync(fileStream);

        var relativePath = Path.Combine("media", path, fileName).Replace("\\", "/");
        return relativePath;
    }

    public async Task DeleteMediaFile(int mediafileId)
    {
        if (mediafileId <= default(int))
        {
            throw new ArgumentNullException(nameof(mediafileId));
        }

        var mediaFile = await _context.MediaFiles.FindAsync(mediafileId);
        if (mediaFile is null)
        {
            throw new NotFoundException();
        }
        
        var relativePath = mediaFile.Path.Replace("media/", string.Empty);
        var fullPath = Path.Combine(_mediaPath, relativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        _context.MediaFiles.Remove(mediaFile);
        await _context.SaveChangesAsync();
    }

    private static string NormalizeAndCombinePaths(string basePath, string relativePath)
    {
        basePath = basePath.Replace('\\', '/');
        relativePath = relativePath.Replace('\\', '/');
        var combinedPath = Path.Combine(basePath, relativePath);

        return Path.GetFullPath(combinedPath);
    }

    private static string GetUniqueFileName(string fileName, bool isProfileImage)
    {
        var extension = Path.GetExtension(fileName);
        var uniqueHash = isProfileImage ? "profile" : GenerateUniqueFileHash(fileName);
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