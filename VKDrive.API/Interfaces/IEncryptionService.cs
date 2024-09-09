namespace VKDrive.API.Interfaces;

/// <summary>
/// Defines methods for file encryption, decryption, and key generation.
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts the specified file using the given key and initialization vector (IV).
    /// </summary>
    /// <param name="filePath">The path to the file that needs to be encrypted.</param>
    /// <param name="key">The encryption key used for the encryption process.</param>
    /// <param name="iv">The initialization vector (IV) used for the encryption process.</param>
    /// <param name="encryptedFilePath">The path where the encrypted file will be saved.</param>
    void EncryptFile(string filePath, byte[] key, byte[] iv, string encryptedFilePath);

    /// <summary>
    /// Decrypts the specified encrypted file using the given key and initialization vector (IV).
    /// </summary>
    /// <param name="encryptedFilePath">The path to the encrypted file.</param>
    /// <param name="key">The encryption key used for decryption.</param>
    /// <param name="iv">The initialization vector (IV) used for decryption.</param>
    /// <param name="decryptedOutputFile">The path where the decrypted file will be saved.</param>
    void DecryptFile(string encryptedFilePath, byte[] key, byte[] iv, string decryptedOutputFile);

    /// <summary>
    /// Generates a secure encryption key of the specified size.
    /// </summary>
    /// <param name="size">The size of the encryption key in bytes.</param>
    /// <returns>A byte array containing the generated encryption key.</returns>
    byte[] GenerateEncryptionKey(int size);
}