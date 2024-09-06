using System.Security.Cryptography;
using VKDrive.API.Interfaces;

namespace VKDrive.API.Services;

// TODO Update the summary
/// <summary>
/// Provides AES encryption services for files and key generation.
/// </summary>
public class AesEncryptionService : IEncryptionService
{
    public void DecryptFile(string encryptedFilePath, byte[] key, byte[] initializationVector, string decryptedOutputFile)
    {
        using var aes = System.Security.Cryptography.Aes.Create();
        aes.Key = key;
        aes.IV = initializationVector;

        Directory.CreateDirectory(Path.GetDirectoryName(decryptedOutputFile));

        using var inputFile = File.OpenRead(encryptedFilePath);
        using var outputFile = File.Create(decryptedOutputFile);
        using var decryptor = aes.CreateDecryptor();
        using var cryptoStream = new CryptoStream(inputFile, decryptor, CryptoStreamMode.Read);
        cryptoStream.CopyTo(outputFile);
    }

    /// <summary>
    /// Encrypts the specified file using AES encryption with the provided key and initialization vector (IV).
    /// </summary>
    /// <param name="filePath">The path to the file that needs to be encrypted.</param>
    /// <param name="key">The encryption key used for the AES encryption process. The size must match the AES algorithm's requirements.</param>
    /// <param name="iv">The initialization vector (IV) used for the AES encryption process. The size must match the AES algorithm's requirements.</param>
    /// <param name="encryptedFilePath">The path where the encrypted file will be saved.</param>
    /// <remarks>
    /// This method performs encryption using the AES (Advanced Encryption Standard) algorithm in CBC mode.
    /// Ensure that the provided key and IV sizes are appropriate for AES, typically 16, 24, or 32 bytes for the key
    /// and 16 bytes for the IV.
    /// </remarks>
    /// <exception cref="IOException">Thrown when an I/O error occurs during file operations.</exception>
    public void EncryptFile(string filePath, byte[] key, byte[] iv, string encryptedFilePath)
    {
        using var aes = System.Security.Cryptography.Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var inputFile = File.OpenRead(filePath);
        using var outputFile = File.Create(encryptedFilePath);
        using var encryptor = aes.CreateEncryptor();
        using var cryptoStream = new CryptoStream(outputFile, encryptor, CryptoStreamMode.Write);
        inputFile.CopyTo(cryptoStream);
    }

    /// <summary>
    /// Generates a secure random encryption key of the specified size.
    /// </summary>
    /// <param name="size">The size of the encryption key in bytes.</param>
    /// <returns>A byte array containing the generated encryption key.</returns>
    /// <remarks>
    /// The generated key is suitable for use in symmetric encryption algorithms, including AES.
    /// </remarks>
    public byte[] GenerateEncryptionKey(int size)
    {
        var key = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(key);

        return key;
    }
}