using SharedEntities.Models;
using VKDrive.API.Interfaces;

namespace VKDrive.API.Services;

public class VkdriveEntryService
{
    private readonly int _archivePasswordLength = 8192;
    private readonly string _temporaryFolder = @"D:\VKDrive\tmp"; // TODO
    private readonly string VKDriveFolder = @"D:\VKDrive"; // TODO
    private readonly IEncryptionService _encryptionService;
    private readonly IArchiveService _zipService;
    private readonly ILogger<VkdriveEntryService> _logger;
    private readonly IVkApiService _vkApiService;
    private readonly IHashingService _hashingService;
    private readonly IVkdriveEntryRepository _repository;
    private readonly FilePartitionerService _filePartitionerService;
    private readonly PasswordGeneratorService _passwordGenerator;

    public VkdriveEntryService(
        FilePartitionerService filePartitionerService,
        PasswordGeneratorService passwordGenerator,
        IEncryptionService encryptionService,
        IVkdriveEntryRepository repository,
        IHashingService hashingService,
        IVkApiService vkApiService,
        IArchiveService zipService,
        ILogger<VkdriveEntryService> logger)
    {
        Guard.AgainstNull(filePartitionerService, nameof(filePartitionerService));
        Guard.AgainstNull(encryptionService, nameof(encryptionService));
        Guard.AgainstNull(passwordGenerator, nameof(passwordGenerator));
        Guard.AgainstNull(hashingService, nameof(hashingService));
        Guard.AgainstNull(vkApiService, nameof(vkApiService));
        Guard.AgainstNull(repository, nameof(repository));
        Guard.AgainstNull(zipService, nameof(zipService));
        Guard.AgainstNull(logger, nameof(logger));

        _filePartitionerService = filePartitionerService;
        _encryptionService = encryptionService;
        _passwordGenerator = passwordGenerator;
        _hashingService = hashingService;
        _vkApiService = vkApiService;
        _repository = repository;
        _zipService = zipService;
        _logger = logger;
    }

    /// <summary>
    /// Creates and saves a new entry for a file or folder by compressing, encrypting, and uploading it to VK server.
    /// The metadata such as hashsum or AES key are gathered and saved in the database.
    /// </summary>
    /// <param name="path">The path of the file or folder to be processed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the specified path does not exist or is not valid.</exception>    
    public async Task CreateAndSaveEntryAsync(string path)
    {
        Guard.AgainstNullOrWhitespace(path, nameof(path));
        Guard.AgainstInvalidPath(path, nameof(path));

        _logger.LogInformation("CreateAndSaveEntryAsync started with path: {Path}", path);

        var isFolder = Directory.Exists(path);
        _logger.LogInformation("Path determined as {Type}: {Path}", isFolder ? "Folder" : "File", path);

        var uniqueName = GenerateUniqueName();
        _logger.LogDebug("Generated unique name: {UniqueName}", uniqueName);

        var sha256Checksum = isFolder 
            ? _hashingService.CalculateFolderHash(path) 
            : _hashingService.CalculateFileHash(path);
        _logger.LogDebug("Calculated checksum: {Checksum}", sha256Checksum);

        var archivePath = Path.Combine(_temporaryFolder, $"{uniqueName}.7z");
        var encryptedFilePath = Path.Combine(_temporaryFolder, $"{sha256Checksum[..12].ToLower()}.aes");
        _logger.LogDebug("Paths prepared: ArchivePath={ArchivePath}, EncryptedFilePath={EncryptedFilePath}", archivePath, encryptedFilePath);
                
        var archivePassword = _passwordGenerator.GeneratePassword(_archivePasswordLength);
        _logger.LogDebug("Generated archive password: {ArchivePassword}", archivePassword);

        var aesKeyLength = 32;
        var aesKey = _encryptionService.GenerateEncryptionKey(aesKeyLength);
        _logger.LogDebug("Generated AES key with {AesKeyLength} length: {AesKey}", aesKeyLength, aesKey);

        var ivLength = 16;
        var initializationVector = _encryptionService.GenerateEncryptionKey(ivLength);
        _logger.LogDebug("Generated initialization vector with {IvLength} length: {initializationVector}", ivLength, initializationVector);

        var initialSize = isFolder ? await GetFolderSize(path) : new FileInfo(path).Length;
        _logger.LogInformation("Initial {Type} size: {Size}", isFolder ? "Folder" : "File", GetReadableSize(initialSize));


        if (isFolder)
        {
            _logger.LogInformation("Compressing folder at {Path} to {ArchivePath} with a password.", path, archivePath);
            _zipService.CompressFolder(path, archivePath, archivePassword);
        }
        else
        {
            _logger.LogInformation("Compressing file at {Path} to {ArchivePath} with a password.", path, archivePath);
            _zipService.CompressFile(path, archivePath, archivePassword);
        }

        var compressedSize = new FileInfo(archivePath).Length;
        _logger.LogInformation("Compressed file size: {Size}", GetReadableSize(compressedSize));

        _logger.LogInformation("Encrypting archive at {ArchivePath} to {EncryptedFilePath}.", archivePath, encryptedFilePath);
        _encryptionService.EncryptFile(archivePath, aesKey, initializationVector, encryptedFilePath);

        var splitFilesFolder = Path.Combine(_temporaryFolder, $"Splitted files {sha256Checksum[..7]}");
        _logger.LogDebug("Splitting encrypted file into pieces in folder: {SplitFilesFolder}", splitFilesFolder);

        var fileParts = SplitFileToPieces(encryptedFilePath, splitFilesFolder, uniqueName[..8]);
        var links = new List<string>();

        _logger.LogInformation("Uploading {FilePartsCount} file parts to VK server.", fileParts.Count);
        foreach (var filePartPath in fileParts)
        {
            var link = await _vkApiService.UploadFileToVkServer(filePartPath);
            links.Add(link);
            _logger.LogDebug("Uploaded file part {FilePartPath} with link: {Link}", filePartPath, link);
        }

        _logger.LogInformation("Cleaning up temporary files.");
        File.Delete(archivePath);
        File.Delete(encryptedFilePath);
        fileParts.ForEach(File.Delete);
        Directory.Delete(splitFilesFolder);

        var entry = new VkdriveEntry
        {
            OriginalName = isFolder ? Path.GetFileName(Path.TrimEndingDirectorySeparator(path)) : Path.GetFileName(path),
            OriginalPath = path,
            UniqueName = uniqueName,
            Checksum = sha256Checksum,
            Size = initialSize,
            ArchivePassword = archivePassword,
            AesKey = aesKey,
            AesIV = initializationVector,
            CreationDate = DateTime.Now,
            Links = links,
            IsFolder = isFolder
        };

        await _repository.AddEntryAsync(entry);
        _logger.LogInformation("Entry successfully created and saved in the database.");
    }

    public async Task<string> GetOriginalFileAsync(string uniqueName)
    {
        var entry = await _repository.GetEntryByUniqueNameAsync(uniqueName);

        if (entry is null)
        {
            _logger.LogWarning("No entry found for unique name: {UniqueName}", uniqueName);
            return $"No entry found with the unique name \"{uniqueName}\".";
        }

        var partsOfFilePaths = await DownloadAllPartsOfFile(entry);
        _logger.LogInformation("Downloaded {Count} parts for the file.", partsOfFilePaths.Count);

        var toBeDecrypted = Path.Combine(_temporaryFolder, "ToBeDecrypted.tmp");
        var archivePath = Path.Combine(_temporaryFolder, "Archive.zip");

        _logger.LogInformation("Joining parts into {ToBeDecrypted}", toBeDecrypted);
        _filePartitionerService.JoinParts(partsOfFilePaths, toBeDecrypted);

        _logger.LogInformation("Deleting temporary files.");
        partsOfFilePaths.ForEach(File.Delete);

        _logger.LogInformation("Decrypting file to {ArchivePath}", archivePath);
        _encryptionService.DecryptFile(toBeDecrypted, entry.AesKey, entry.AesIV, archivePath);

        _logger.LogDebug("Deleting temporary file {ToBeDecrypted}", toBeDecrypted);
        File.Delete(toBeDecrypted);

        _logger.LogInformation("Decompressing archive to {TemporaryFolder}", _temporaryFolder);
        _zipService.DecompressArchive(archivePath, _temporaryFolder, entry.ArchivePassword);

        _logger.LogDebug("Deleting archive file {ArchivePath}", archivePath);
        File.Delete(archivePath);

        _logger.LogInformation("File retrieval successful.");

        return Path.Combine(_temporaryFolder, entry.OriginalName);
    }    

    public async Task<IEnumerable<VkdriveEntry>> GetAllEntriesAsync()
    {
        return await _repository.GetAllEntriesAsync();
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
        var downloadsFolder = Path.Combine(_temporaryFolder, "Downloads");
        if (!Directory.Exists(downloadsFolder))
        {
            Directory.CreateDirectory(downloadsFolder);
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
            var pathToBeWritten = Path.Combine(downloadsFolder, fileName);
            await File.WriteAllBytesAsync(pathToBeWritten, fileBytes);
            downloadedParts.Add(pathToBeWritten);
        }

        return downloadedParts;
    }

    private List<string> SplitFileToPieces(string filePath, string outputDirectory, string outputFileName)
    {
        // TODO: Get from settings!
        var chinkSizeMb = 1;

        return _filePartitionerService.SplitFile(filePath, chinkSizeMb, outputDirectory, outputFileName);
    }    

    private static string GenerateUniqueName()
    {
        // TODO Make SUPER unique
        return Guid.NewGuid().ToString();
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
            return $"{bytes / 1024:0.##} KB";
        }
        else
        {
            return $"{bytes} bytes";
        }
    }
}