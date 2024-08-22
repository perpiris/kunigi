namespace Kunigi.Utilities;

public class CrossPlatformPathUtility
{
    private static string NormalizePath(string path)
    {
        return path.Replace('\\', '/');
    }

    public static string CombineAndNormalize(params string[] paths)
    {
        return NormalizePath(Path.Combine(paths));
    }

    public static string GetRelativePath(string basePath, string fullPath)
    {
        return NormalizePath(Path.GetRelativePath(basePath, fullPath));
    }

    public static string GetAbsolutePath(string basePath, string relativePath)
    {
        return NormalizePath(Path.GetFullPath(Path.Combine(basePath, relativePath)));
    }
}