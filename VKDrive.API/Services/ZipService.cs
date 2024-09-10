using Ionic.Zip;
using System.Text;
using VKDrive.API.Interfaces;

namespace VKDrive.API.Services;

public class ZipService : IArchiveService
{
    public void CompressFile(string fileToCompress, string outputArchive, string password)
    {
        ValidateZipInput(fileToCompress, outputArchive, password);

        // TODO: Hardcoded!
        var level = Ionic.Zlib.CompressionLevel.Default;

        using var zip = new ZipFile()
        {
            UseZip64WhenSaving = Zip64Option.Always,
            Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256,
            Password = password,
            CompressionLevel = level,
            AlternateEncoding = Encoding.UTF8,
            AlternateEncodingUsage = ZipOption.AsNecessary
        };

        zip.AddFile(fileToCompress, string.Empty);
        zip.Save(outputArchive);
    }

    public void CompressFolder(string folderToCompress, string outputArchive, string password)
    {
        ValidateZipInput(folderToCompress, outputArchive, password);

        // TODO: Hardcoded!
        var level = Ionic.Zlib.CompressionLevel.Default;

        using var zip = new ZipFile()
        {
            UseZip64WhenSaving = Zip64Option.Always,
            Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256,
            Password = password,
            CompressionLevel = level,
            AlternateEncoding = Encoding.UTF8,
            AlternateEncodingUsage = ZipOption.AsNecessary
        };

        zip.AddDirectory(folderToCompress, Path.GetFileName(folderToCompress));
        zip.Save(outputArchive);
    }

    public void DecompressArchive(string archiveToDecompress, string outputFolder, string password)
    {
        ValidateZipInput(archiveToDecompress, outputFolder, password);

        using var zip = Ionic.Zip.ZipFile.Read(archiveToDecompress);
        zip.Password = password;
        zip.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
        try
        {
            zip.ExtractAll(outputFolder, ExtractExistingFileAction.OverwriteSilently);
        }
        catch (BadPasswordException e)
        {
            // TODO:
            throw new Exception(e.Message);
        }
        catch (IOException e)
        {
            // TODO:
            throw new Exception(e.Message);
        }
    }

    private void ValidateZipInput(string filePath, string toBeCreated, string password)
    {
        Guard.AgainstInvalidPath(filePath, nameof(filePath));
        Guard.AgainstNullOrWhitespace(toBeCreated, nameof(toBeCreated));
        Guard.AgainstNullOrWhitespace(password, nameof(password));
    }
}
