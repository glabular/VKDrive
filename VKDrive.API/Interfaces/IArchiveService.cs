namespace VKDrive.API.Interfaces;

public interface IArchiveService
{
    void CompressFile(string fileToCompress, string outputArchive, string password);

    void CompressFolder(string folderToCompress, string outputArchive, string password);

    void DecompressArchive(string archiveToDecompress, string outputFolder, string password);
}