namespace VKDrive.API.Interfaces;

/// <summary>
/// Defines methods for compressing and decompressing files and folders into password-protected archives.
/// </summary>
public interface IArchiveService
{
    /// <summary>
     /// Compresses a file into an archive with optional password protection.
     /// </summary>
     /// <param name="fileToCompress">The full path to the file that needs to be compressed.</param>
     /// <param name="outputArchive">The full path where the compressed archive should be created.</param>
     /// <param name="password">The password used to protect the archive. Can be null or empty for no protection.</param>
    void CompressFile(string fileToCompress, string outputArchive, string password);

    /// <summary>
    /// Compresses a folder into an archive with optional password protection.
    /// </summary>
    /// <param name="folderToCompress">The full path to the folder that needs to be compressed.</param>
    /// <param name="outputArchive">The full path where the compressed archive should be created.</param>
    /// <param name="password">The password used to protect the archive. Can be null or empty for no protection.</param>
    void CompressFolder(string folderToCompress, string outputArchive, string password);

    /// <summary>
    /// Decompresses an archive to a specified folder using optional password protection.
    /// </summary>
    /// <param name="archiveToDecompress">The full path to the archive file that needs to be decompressed.</param>
    /// <param name="outputFolder">The folder where the contents of the archive will be extracted.</param>
    /// <param name="password">The password used to decrypt the archive. Can be null or empty if the archive is not password-protected.</param>
    void DecompressArchive(string archiveToDecompress, string outputFolder, string password);
}