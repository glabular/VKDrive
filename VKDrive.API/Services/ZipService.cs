using Ionic.Zip;
using SharedEntities;
using SharedEntities.Models;
using SharedEntities.Settings;
using System.Text;
using VKDrive.API.Interfaces;

namespace VKDrive.API.Services;

public class ZipService : IArchiveService
{
    private readonly Settings _settings;

    public ZipService()
    {
        _settings = SettingsManager.LoadSettings();
        Guard.AgainstNull(_settings, nameof(_settings));
    }

    public void CompressFile(string fileToCompress, string outputArchive, string password)
    {
        ValidateZipInput(fileToCompress, outputArchive, password);

        var level = GetCompressionLevel();

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

        var level = GetCompressionLevel();

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
        catch (BadPasswordException)
        {
            throw;
        }
        catch (IOException)
        {
            throw;
        }
    }

    private Ionic.Zlib.CompressionLevel GetCompressionLevel()
    {
        var selectedLevel = _settings.CompressionLevel;

        return selectedLevel switch
        {
            MyCompressionLevel.None => Ionic.Zlib.CompressionLevel.None,
            MyCompressionLevel.Minimal => Ionic.Zlib.CompressionLevel.Level3,
            MyCompressionLevel.Default => Ionic.Zlib.CompressionLevel.Default,
            MyCompressionLevel.Best => Ionic.Zlib.CompressionLevel.BestCompression,
            _ => throw new ArgumentException("Invalid compression level"),
        };
    }

    private static void ValidateZipInput(string filePath, string toBeCreatedFilePath, string password)
    {
        Guard.AgainstInvalidPath(filePath, nameof(filePath));
        Guard.AgainstNullOrWhitespace(toBeCreatedFilePath, nameof(toBeCreatedFilePath));
        Guard.AgainstNullOrWhitespace(password, nameof(password));
    }
}
