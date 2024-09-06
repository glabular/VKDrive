namespace VKDrive.API.Interfaces;

public interface IVkApiService
{
    Task<string> UploadFileToVkServer(string filePath);

    Task<string> DeleteDocumentFromVkServer(string docUrl);
}
