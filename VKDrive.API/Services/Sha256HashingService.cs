using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using VKDrive.API.Interfaces;

namespace VKDrive.API.Services;

public class Sha256HashingService : IHashingService
{
    /// <summary>
    /// Calculates the SHA-256 hash of a file at the specified path.
    /// </summary>
    /// <param name="filePath">The path of the file to hash.</param>
    /// <returns>The SHA-256 hash of the file as a hexadecimal string.</returns>
    public string CalculateFileHash(string filePath)
    {
        return CalculateSha256Checksum(filePath);
    }

    /// <summary>
    /// Calculates the SHA-256 hash of a folder and its contents.
    /// </summary>
    /// <param name="folderPath">The path of the folder to hash.</param>
    /// <returns>The SHA-256 hash of the folder and its contents as a hexadecimal string.</returns>
    public string CalculateFolderHash(string folderPath)
    {
        return CalculateFolderSha256Checksum(folderPath);
    }

    /// <summary>
    /// Calculates the SHA-256 checksum of a file at the specified path.
    /// </summary>
    /// <param name="filePath">The path of the file to hash.</param>
    /// <returns>The SHA-256 checksum as a hexadecimal string.</returns>
    /// <exception cref="IOException">Thrown when there is an issue reading the file.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when access to the file is denied.</exception>
    private static string CalculateSha256Checksum(string filePath)
    {
        try
        {
            using var sha256 = SHA256.Create();
            using FileStream fileStream = File.OpenRead(filePath);
            fileStream.Position = 0;
            var hashValue = sha256.ComputeHash(fileStream);

            return ByteArrayToString(hashValue);
        }
        catch (IOException e)
        {
            throw new Exception($"I/O Exception: {e.Message}");
        }
        catch (UnauthorizedAccessException e)
        {
            throw new Exception($"Access Exception: {e.Message}");
        }
    }

    /// <summary>
    /// Computes the SHA-256 hash of all files within a folder.
    /// </summary>
    /// <param name="folderPath">The path of the folder to hash.</param>
    /// <returns>The combined SHA-256 hash of all files in the folder as a hexadecimal string.</returns>
    private static string CalculateFolderSha256Checksum(string folderPath)
    {
        using var sha256 = SHA256.Create();
        var files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
        var checksums = new List<string>();

        foreach (var file in files)
        {
            try
            {
                var fileChecksum = CalculateSha256Checksum(file);
                checksums.Add(fileChecksum);
            }
            catch (Exception ex)
            {
                // Log
            }
        }

        var folderChecksum = CalculateCombinedChecksum(checksums);

        return folderChecksum;
    }

    /// <summary>
    /// Combines a list of individual checksums into a single SHA-256 checksum.
    /// </summary>
    /// <param name="checksums">A list of individual file checksums.</param>
    /// <returns>The combined checksum as a hexadecimal string.</returns>
    private static string CalculateCombinedChecksum(List<string> checksums)
    {
        // Combine the individual checksums into one string
        var combinedChecksum = string.Join("", checksums);

        // Compute the hash of the combined checksum string
        using var sha256 = SHA256.Create();
        var combinedChecksumBytes = Encoding.UTF8.GetBytes(combinedChecksum);
        var finalHash = sha256.ComputeHash(combinedChecksumBytes);

        return ByteArrayToString(finalHash);
    }

    /// <summary>
    /// Converts a byte array into a hexadecimal string representation.
    /// </summary>
    /// <param name="array">The byte array to convert.</param>
    /// <returns>A string representing the hexadecimal values of the bytes in the array.</returns>
    /// <remarks>
    /// Each byte in the array is converted to a two-character hexadecimal string, and these strings are concatenated
    /// to form the final output string. This method is typically used to represent binary data in a human-readable format.
    /// </remarks>
    private static string ByteArrayToString(byte[] array)
    {
        StringBuilder sb = new();

        for (int i = 0; i < array.Length; i++)
        {
            sb.Append($"{array[i]:X2}");
        }

        return sb.ToString();
    }
}