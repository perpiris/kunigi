namespace Kunigi.Services;

public interface IMediaService
{
    void CreateFolder(string path);

    Task<string> SaveMediaFile(IFormFile file, string path);

    Task DeleteMediaFile(Guid mediafileId);
}