namespace VKDrive.API.Interfaces;

/// <summary>
/// Provides methods for interacting with the VK server for file upload and deletion.
/// </summary>
public interface IVkApiService
{
    /// <summary>
    /// Uploads a file to the VK server.
    /// </summary>
    /// <param name="filePath">The path to the file that needs to be uploaded.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains the URL of the uploaded file.</returns>
    Task<string> UploadFileToVkServer(string filePath);

    /// <summary>
    /// Deletes a document from the VK server using its URL.
    /// </summary>
    /// <param name="docUrl">The URL of the document to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a response indicating the success or failure of the deletion.</returns>
    Task<string> DeleteDocumentFromVkServer(string docUrl);
}
