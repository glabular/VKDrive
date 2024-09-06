using System.Security.Cryptography;

namespace VKDrive.API.Services;

public class PasswordGeneratorService
{
    /// <summary>
    /// Generates a random password.
    /// </summary>
    /// <param name="length">The length of the password to be generated.</param>
    /// <returns>A random password string of the specified length.</returns>
    /// <remarks>
    /// This method uses a cryptographically secure random number generator to ensure that the password
    /// generated is secure and difficult to guess or predict. The characters used for password generation
    /// include lowercase and uppercase letters, digits, and special characters to increase the complexity.
    /// </remarks>
    public string GeneratePassword(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-_=+[{]}|;:',<.>/?`~";
        var password = new char[length];
        var rng = RandomNumberGenerator.Create();
        var buffer = new byte[sizeof(uint)];

        for (int i = 0; i < length; i++)
        {
            rng.GetBytes(buffer);
            uint randomIndex = BitConverter.ToUInt32(buffer, 0) % (uint)chars.Length;
            password[i] = chars[(int)randomIndex];
        }

        return new string(password);
    }
}