namespace VKDrive.API.Interfaces;

/// <summary>
/// Provides methods for calculating the hash sums of files and folders.
/// </summary>
public interface IHashingService
{
    /// <summary>
    /// Calculates the hash sum of a single file.
    /// </summary>
    /// <param name="filePath">The path of the file to hash.</param>
    /// <returns>The hash sum as a hexadecimal string.</returns>
    string CalculateFileHash(string filePath);

    /// <summary>
    /// Calculates the hash sum of a folder.
    /// </summary>
    /// <param name="folderPath">The path of the folder to hash.</param>
    /// <returns>The hash sum of a folder as a hexadecimal string.</returns>
    string CalculateFolderHash(string folderPath);
}