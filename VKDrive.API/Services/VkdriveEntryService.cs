using SharedEntities;
using SharedEntities.Models;
using SharedEntities.Settings;
using System.Security.Cryptography;
using VKDrive.API.Interfaces;

namespace VKDrive.API.Services;

public class VkdriveEntryService
{
    private readonly string VKDriveFolder = @"D:\VKDrive"; // TODO
    private readonly string _temporaryFolder = @"D:\VKDrive\tmp"; // TODO
    private readonly string _downloadsFolder = @"D:\VKDrive\tmp\Downloads"; // TODO
    private const int _archivePasswordLength = 8192; // TODO
    private const int _aesKeyLength = 32;
    private const int _ivLength = 16;
    private readonly FilePartitionerService _filePartitionerService;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<VkdriveEntryService> _logger;
    private readonly IVkdriveEntryRepository _repository;
    private readonly IHashingService _hashingService;
    private readonly IArchiveService _zipService;
    private readonly IVkApiService _vkApiService;
    private readonly Settings _settings;

    public VkdriveEntryService(
        FilePartitionerService filePartitionerService,
        IEncryptionService encryptionService,
        IVkdriveEntryRepository repository,
        IHashingService hashingService,
        IVkApiService vkApiService,
        IArchiveService zipService,
        ILogger<VkdriveEntryService> logger)
    {
        Guard.AgainstNull(filePartitionerService, nameof(filePartitionerService));
        Guard.AgainstNull(encryptionService, nameof(encryptionService));
        Guard.AgainstNull(hashingService, nameof(hashingService));
        Guard.AgainstNull(vkApiService, nameof(vkApiService));
        Guard.AgainstNull(repository, nameof(repository));
        Guard.AgainstNull(zipService, nameof(zipService));
        Guard.AgainstNull(logger, nameof(logger));

        _filePartitionerService = filePartitionerService;
        _encryptionService = encryptionService;
        _hashingService = hashingService;
        _vkApiService = vkApiService;
        _repository = repository;
        _zipService = zipService;
        _logger = logger;

        _settings = SettingsManager.LoadSettings();
        Guard.AgainstNull(_settings, nameof(_settings));
    }

    public async Task<IEnumerable<VkdriveEntry>> GetAllEntriesAsync() => await _repository.GetAllEntriesAsync();

    /// <summary>
    /// Creates and saves a new entry for a file or folder by compressing, encrypting, and uploading it to VK server.
    /// The metadata such as hashsum or AES key are gathered and saved in the database.
    /// </summary>
    /// <param name="path">The path of the file or folder to be processed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the specified path does not exist or is not valid.</exception>    
    public async Task CreateAndSaveEntryAsync(string path)
    {
        Guard.AgainstInvalidPath(path, nameof(path));

        _logger.LogInformation("Entry creation process started for file: {FilePath}", path);

        var isFolder = IsFolder(path);
        var uniqueName = GenerateUniqueName();
        var sha256Checksum = CalculateChecksum(path, isFolder);
        var archivePath = Path.Combine(_temporaryFolder, $"{uniqueName}.7z");
        var encryptedFilePath = Path.Combine(_temporaryFolder, $"{sha256Checksum[..12].ToLower()}.aes");
        var archivePassword = GenerateArchivePassword();
        var initialSize = await CalculateInitialSize(path, isFolder);

        PerformCompressing(path, isFolder, archivePath, archivePassword);

        var aesKey = GenerateAesKey();
        var initializationVector = GenerateIV();

        PerformEncryption(archivePath, encryptedFilePath, aesKey, initializationVector);

        var splitFilesFolder = Path.Combine(_temporaryFolder, $"Splitted files {sha256Checksum[..7]}");        
        var fileParts = PerformFileSplitting(uniqueName, encryptedFilePath, splitFilesFolder);        
        var links = await UploadFilePartsToServer(fileParts);

        PerformCleanup(archivePath, encryptedFilePath, splitFilesFolder, fileParts);

        var entry = new VkdriveEntry
        {
            AesIV = initializationVector,
            AesKey = aesKey,
            ArchivePassword = archivePassword,
            Checksum = sha256Checksum,
            CreationDate = DateTime.Now,
            IsFolder = isFolder,
            Links = links,
            OriginalName = isFolder ? new DirectoryInfo(path).Name : Path.GetFileName(path),
            OriginalPath = path,
            Size = initialSize,
            UniqueName = uniqueName,
        };

        await _repository.AddEntryAsync(entry);
        _logger.LogInformation("Entry successfully created and saved in the database.");
    }

    public async Task<string> GetOriginalFileAsync(string uniqueName)
    {
        // TODO: Log trash fies ON DISK to clean them up even when the programm crashes.

        var entry = await _repository.GetEntryByUniqueNameAsync(uniqueName);

        if (entry is null)
        {
            _logger.LogWarning("No entry found for unique name: {UniqueName}", uniqueName);
            return $"No entry found with the unique name \"{uniqueName}\".";
        }

        var partsOfFilePaths = await DownloadAllPartsOfFile(entry);        

        var toBeDecrypted = Path.Combine(_temporaryFolder, "ToBeDecrypted.tmp");
        var archivePath = Path.Combine(_temporaryFolder, "Archive.zip");

        _logger.LogInformation("Joining parts into {ToBeDecrypted}", toBeDecrypted);
        _filePartitionerService.JoinParts(partsOfFilePaths, toBeDecrypted);

        _logger.LogInformation("Deleting temporary files.");
        partsOfFilePaths.ForEach(File.Delete);

        _logger.LogInformation("Decrypting file to {ArchivePath}", archivePath);
        _encryptionService.DecryptFile(toBeDecrypted, entry.AesKey, entry.AesIV, archivePath);

        _logger.LogInformation("Deleting temporary file {ToBeDecrypted}", toBeDecrypted);
        File.Delete(toBeDecrypted);

        if (!Directory.Exists(_downloadsFolder))
        {
            Directory.CreateDirectory(_downloadsFolder);
        }

        _logger.LogInformation("Decompressing archive to {DownloadsFolder}", _downloadsFolder);
        _zipService.DecompressArchive(archivePath, _downloadsFolder, entry.ArchivePassword);

        _logger.LogInformation("Deleting archive file {ArchivePath}", archivePath);
        File.Delete(archivePath);

        _logger.LogInformation("File retrieval successful.");

        return Path.Combine(_downloadsFolder, entry.OriginalName);
    }

    public async Task<string> DeleteEntryAsync(string uniqueName)
    {
        if (string.IsNullOrEmpty(uniqueName))
        {
            _logger.LogWarning("DeleteEntryAsync was called with a null or empty unique name.");

            return "Unique name cannot be null or empty.";
        }

        _logger.LogInformation("DeleteEntryAsync started for unique name: {UniqueName}", uniqueName);
        _logger.LogDebug("Attempting to retrieve entry for unique name: {UniqueName}", uniqueName);
        var entry = await _repository.GetEntryByUniqueNameAsync(uniqueName);

        if (entry is null)
        {
            _logger.LogWarning("No entry found with the unique name \"{UniqueName}\".", uniqueName);

            return $"No entry found with the unique name \"{uniqueName}\".";
        }

        _logger.LogInformation("Entry found. Proceeding to delete file parts from VK server.");
        await DeleteFilePartsFromVkServer(entry.Links);

        _logger.LogInformation("File parts deleted. Proceeding to remove entry from repository.");
        await _repository.DeleteEntryAsync(uniqueName);

        _logger.LogInformation("Deletion successful for entry with unique name: {UniqueName}", uniqueName);

        return "Deletion successful.";
    }

    private bool IsFolder(string path)
    {
        var isFolder = Directory.Exists(path);
        _logger.LogInformation("Path determined as {Type}: {Path}", isFolder ? "Folder" : "File", path);
        return isFolder;
    }

    private async Task<List<string>> UploadFilePartsToServer(List<string> fileParts)
    {
        _logger.LogInformation("Uploading {FilePartsCount} file part(s) to VK server.", fileParts.Count);

        var URLs = new List<string>();

        foreach (var filePartPath in fileParts)
        {
            var url = await _vkApiService.UploadFileToVkServer(filePartPath);
            URLs.Add(url);
            _logger.LogDebug("Uploaded file part {FilePartPath} with URL: {URL}", filePartPath, url);
        }

        return URLs;
    }

    private List<string> PerformFileSplitting(string uniqueName, string filePath, string splitFilesFolder)
    {
        _logger.LogDebug("Splitting encrypted file into chunks in folder: {SplitFilesFolder}", splitFilesFolder);

        var chinkSizeMb = _settings.ChunkToUploadSizeInMegabytes;

        return _filePartitionerService.SplitFile(filePath, chinkSizeMb, splitFilesFolder, uniqueName[..8]);
    }

    private void PerformEncryption(string archivePath, string encryptedFilePath, byte[] aesKey, byte[] initializationVector)
    {
        _logger.LogInformation("Encrypting archive at {ArchivePath} to {EncryptedFilePath}.", archivePath, encryptedFilePath);
        _encryptionService.EncryptFile(archivePath, aesKey, initializationVector, encryptedFilePath);
    }

    private byte[] GenerateIV()
    {
        var initializationVector = _encryptionService.GenerateEncryptionKey(_ivLength);
        _logger.LogDebug("Generated initialization vector with {IvLength} length: {initializationVector}", _ivLength, initializationVector);
        return initializationVector;
    }

    private byte[] GenerateAesKey()
    {
        var aesKey = _encryptionService.GenerateEncryptionKey(_aesKeyLength);
        _logger.LogDebug("Generated AES key with {AesKeyLength} length: {AesKey}", _aesKeyLength, aesKey);
        return aesKey;
    }

    private async Task<long> CalculateInitialSize(string path, bool isFolder)
    {
        var initialSize = isFolder ? await GetFolderSize(path) : new FileInfo(path).Length;
        _logger.LogInformation("Initial {Type} size: {Size}", isFolder ? "Folder" : "File", GetReadableSize(initialSize));
        return initialSize;
    }

    private string GenerateArchivePassword()
    {
        // TODO: Refactor, use GenerateEncryptionKey from AesEncryptionService.
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-_=+[{]}|;:',<.>/?`~";
        var password = new char[_archivePasswordLength];
        var rng = RandomNumberGenerator.Create();
        var buffer = new byte[sizeof(uint)];

        for (int i = 0; i < _archivePasswordLength; i++)
        {
            rng.GetBytes(buffer);
            uint randomIndex = BitConverter.ToUInt32(buffer, 0) % (uint)chars.Length;
            password[i] = chars[(int)randomIndex];
        }

        var archivePassword = new string(password);

        _logger.LogDebug("Generated archive password: {ArchivePassword}", archivePassword);

        return archivePassword;
    }

    private string CalculateChecksum(string path, bool isFolder)
    {
        var sha256Checksum = isFolder
            ? _hashingService.CalculateFolderHash(path)
            : _hashingService.CalculateFileHash(path);
        _logger.LogDebug("Calculated checksum: {Checksum}", sha256Checksum);

        return sha256Checksum;
    }

    private void PerformCleanup(string archivePath, string encryptedFilePath, string splitFilesFolder, List<string> fileParts)
    {
        _logger.LogInformation("Cleaning up temporary files.");

        _logger.LogInformation("Deleting archive file: {ArchivePath}", archivePath);
        File.Delete(archivePath);

        _logger.LogInformation("Deleting encrypted file: {EncryptedFilePath}", encryptedFilePath);
        File.Delete(encryptedFilePath);

        _logger.LogInformation("Deleting file parts.");
        foreach (var filePart in fileParts)
        {
            _logger.LogInformation("Deleting file part: {FilePart}", filePart);
            File.Delete(filePart);
        }

        _logger.LogInformation("Deleting split files folder: {SplitFilesFolder}", splitFilesFolder);
        Directory.Delete(splitFilesFolder);

        _logger.LogInformation("Cleanup completed.");
    }

    private void PerformCompressing(string path, bool isFolder, string archivePath, string archivePassword)
    {
        if (isFolder)
        {
            _logger.LogInformation("Compressing folder at {Path} to {ArchivePath} with {PasswordLength}-length password.", path, archivePath, archivePassword.Length);
            _zipService.CompressFolder(path, archivePath, archivePassword);
        }
        else
        {
            _logger.LogInformation("Compressing file at {Path} to {ArchivePath} with {PasswordLength}-length password.", path, archivePath, archivePassword.Length);
            _zipService.CompressFile(path, archivePath, archivePassword);
        }

        var compressedSize = new FileInfo(archivePath).Length;
        _logger.LogInformation("Compressed archive size: {Size}", GetReadableSize(compressedSize));
    }

    private async Task DeleteFilePartsFromVkServer(List<string> links)
    {
        var linkCount = links?.Count ?? 0;
        _logger.LogInformation("DeleteFilePartsFromVkServer started. Number of file parts to delete: {LinkCount}", linkCount);

        if (links is null || links.Count == 0)
        {
            _logger.LogWarning("No links provided to delete files from VK server.");

            return;
        }

        foreach (var link in links)
        {
            _logger.LogDebug("Attempting to delete file part with link: {Link}", link);

            try
            {
                await _vkApiService.DeleteDocumentFromVkServer(link);
                _logger.LogInformation("Successfully deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file part with link: {Link}", link);
            }
        }

        _logger.LogInformation("DeleteFilePartsFromVkServer completed. Total file parts deleted from the server: {LinkCount}", linkCount);
    }

    private async Task<List<string>> DownloadAllPartsOfFile(VkdriveEntry entry)
    {
        var downloadedParts = new List<string>();
        if (!Directory.Exists(_downloadsFolder))
        {
            Directory.CreateDirectory(_downloadsFolder);
        }

        using var client = new HttpClient();

        var counter = 0;
        foreach (var url in entry.Links)
        {
            counter++;
            using var response = await client.GetAsync(url);
            using var content = response.Content;
            var fileBytes = await content.ReadAsByteArrayAsync();
            var fileName = $"{entry.OriginalName} - to be assembled-{counter}.vkd";
            var pathToBeWritten = Path.Combine(_downloadsFolder, fileName);
            await File.WriteAllBytesAsync(pathToBeWritten, fileBytes);
            downloadedParts.Add(pathToBeWritten);
        }

        _logger.LogInformation("Downloaded {Count} parts for the file.", downloadedParts.Count);

        return downloadedParts;
    }    

    private string GenerateUniqueName()
    {
        var uniqueName = $"{Guid.NewGuid()}{Guid.NewGuid()}";
        _logger.LogDebug("Generated unique name: {UniqueName}", uniqueName);

        return uniqueName;
    }

    private static async Task<long> GetFolderSize(string folderPath)
    {
        var directoryInfo = new DirectoryInfo(folderPath);
        long totalSize = 0;

        var filesList = await Task.Run(() =>
        {
            return directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
        });

        foreach (var fileInfo in filesList)
        {
            totalSize += fileInfo.Length;
        }

        return totalSize;
    }

    /// <summary>
    /// Converts the file size from bytes to a human-readable format.
    /// </summary>
    public string GetReadableSize(long bytes)
    {
        if (bytes >= 1024 * 1024 * 1024)
        {
            return $"{bytes / (1024 * 1024 * 1024):0.##} GB";
        }
        else if (bytes >= 1024 * 1024)
        {
            return $"{bytes / (1024 * 1024):0.##} MB";
        }
        else if (bytes >= 1024)
        {
            return $"{bytes / 1024:0.##} kB";
        }
        else
        {
            return $"{bytes} bytes";
        }
    }
}
