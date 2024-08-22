namespace Kunigi.Utilities;

public class CrossPlatformPathUtility
{
    public static string CombineAndNormalize(params string[] paths)
    {
        return NormalizePath(Path.Combine(paths));
    }

    private static string NormalizePath(string path)
    {
        return path.Replace('\\', '/').TrimEnd('/');
    }

    public static string GetRelativePath(string basePath, string fullPath)
    {
        return NormalizePath(Path.GetRelativePath(basePath, fullPath));
    }
}