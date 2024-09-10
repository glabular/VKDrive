using System.Security.Cryptography;
using VKDrive.API.Interfaces;

namespace VKDrive.API.Services;

/// <summary>
/// Provides AES (Advanced Encryption Standard) encryption and decryption services. 
/// </summary>
public class AesEncryptionService : IEncryptionService
{
    /// <summary>
    /// Encrypts the specified file using AES encryption with the provided key and initialization vector (IV).
    /// </summary>
    /// <param name="filePath">The path to the file that needs to be encrypted.</param>
    /// <param name="key">
    /// The encryption key used for encryption. The key must be either 16, 24, or 32 bytes in length, corresponding to AES key sizes of 128, 192, or 256 bits.
    /// </param>
    /// <param name="iv">
    /// The initialization vector (IV) used for encryption. The IV must be exactly 16 bytes in length (128 bits), as required by AES encryption.
    /// </param>
    /// <param name="encryptedFilePath">The path where the encrypted file will be saved.</param>
    /// <remarks>
    /// This method performs encryption using the AES (Advanced Encryption Standard) algorithm in CBC mode.
    /// </remarks>
    /// <exception cref="IOException">Thrown when an I/O error occurs during file operations.</exception>
    public void EncryptFile(string filePath, byte[] key, byte[] iv, string encryptedFilePath)
    {
        ValidateEncryptionInput(filePath, key, iv, encryptedFilePath);

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
    /// Decrypts the specified encrypted file using AES with the provided key and initialization vector (IV).
    /// </summary>
    /// <param name="encryptedFilePath">The path to the encrypted file that needs to be decrypted.</param>
    /// <param name="key">
    /// The encryption key used for decryption. The key must be either 16, 24, or 32 bytes in length, corresponding to AES key sizes of 128, 192, or 256 bits.
    /// </param>
    /// <param name="iv">
    /// The initialization vector (IV) used for decryption. The IV must be exactly 16 bytes in length (128 bits), as required by AES encryption.
    /// </param>
    /// <param name="decryptedOutputFile">The path where the decrypted file will be saved.</param>
    /// <exception cref="ArgumentException">Thrown if the encryption key or IV length is invalid for AES decryption.</exception>
    /// <exception cref="ArgumentNullException">Thrown if any of the required inputs are null or empty.</exception>
    /// <exception cref="IOException">Thrown when an I/O error occurs during file operations.</exception>
    public void DecryptFile(string encryptedFilePath, byte[] key, byte[] iv, string decryptedOutputFile)
    {
        ValidateDecryptionInput(encryptedFilePath, key, iv, decryptedOutputFile);

        using var aes = System.Security.Cryptography.Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        Directory.CreateDirectory(Path.GetDirectoryName(decryptedOutputFile));

        using var inputFile = File.OpenRead(encryptedFilePath);
        using var outputFile = File.Create(decryptedOutputFile);
        using var decryptor = aes.CreateDecryptor();
        using var cryptoStream = new CryptoStream(inputFile, decryptor, CryptoStreamMode.Read);
        cryptoStream.CopyTo(outputFile);
    }

    /// <summary>
    /// Generates a secure random encryption key of the specified size.
    /// </summary>
    /// <param name="keySize">The size of the encryption key in bytes.</param>
    /// <returns>A byte array containing the generated encryption key.</returns>
    public byte[] GenerateEncryptionKey(int keySize)
    {
        if (keySize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(keySize), "Key size must be a positive integer.");
        }

        var key = new byte[keySize];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(key);

        return key;
    }

    /// <summary>
    /// Validates the input parameters for the decryption process, ensuring the file paths, encryption key, and initialization vector (IV) are valid.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if the encryption key or IV length is invalid for AES encryption.</exception>
    /// <exception cref="ArgumentNullException">Thrown if any of the required inputs are null or empty.</exception>
    /// <exception cref="ArgumentException">Thrown if the file paths provided are invalid.</exception>
    private static void ValidateDecryptionInput(string encryptedFilePath, byte[] key, byte[] iv, string decryptedOutputFile)
    {
        Guard.AgainstNullOrWhitespace(decryptedOutputFile, nameof(decryptedOutputFile));
        Guard.AgainstInvalidPath(encryptedFilePath, nameof(encryptedFilePath));
        Guard.AgainstNullOrEmptyCollection(key, nameof(key));
        Guard.AgainstNullOrEmptyCollection(iv, nameof(iv));

        // AES encryption requires key lengths of 16, 24, or 32 bytes (128, 192, or 256 bits)
        if (key.Length != 32 && key.Length != 24 && key.Length != 16)
        {
            throw new ArgumentException("Invalid AES encryption key length. Key length must be 16, 24, or 32 bytes (128, 192, or 256 bits).", nameof(key));
        }

        // AES encryption requires an IV length of 16 bytes (128 bits)
        if (iv.Length != 16)
        {
            throw new ArgumentException("Invalid AES initialization vector (IV) length. IV length must be exactly 16 bytes (128 bits).", nameof(iv));
        }
    }

    /// <summary>
    /// Validates the input parameters for the encryption process, ensuring the file paths, encryption key, and initialization vector (IV) are valid.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if any of the required inputs are null or empty.</exception>
    /// <exception cref="ArgumentException">Thrown if the file paths provided are invalid.</exception>
    private static void ValidateEncryptionInput(string filePath, byte[] key, byte[] iv, string encryptedFilePath)
    {
        Guard.AgainstNullOrWhitespace(encryptedFilePath, nameof(encryptedFilePath));
        Guard.AgainstInvalidPath(filePath, nameof(filePath));
        Guard.AgainstNullOrEmptyCollection(key, nameof(key));
        Guard.AgainstNullOrEmptyCollection(iv, nameof(iv));
    }
}
