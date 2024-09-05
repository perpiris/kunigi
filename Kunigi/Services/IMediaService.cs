namespace Kunigi.Services;

public interface IMediaService
{
    string CreateFolder(string path);

    Task<string> SaveMediaFile(IFormFile file, string path, bool isProfileImage);

    Task DeleteMediaFile(int mediafileId);
}