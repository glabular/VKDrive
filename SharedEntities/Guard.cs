namespace SharedEntities;

public static class Guard
{
    public static void AgainstNull<T>(T value, string paramName) where T : class
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");
        }
    }

    public static void AgainstNullOrEmptyCollection<T>(IEnumerable<T> collection, string paramName)
    {
        if (collection is null || !collection.Any())
        {
            throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);
        }
    }

    public static void AgainstNullOrWhitespace(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{paramName} cannot be null or whitespace.", paramName);
        }
    }

    public static void AgainstInvalidPath(string path, string paramName)
    {
        var isValidPath = Directory.Exists(path) || File.Exists(path);

        if (!isValidPath)
        {
            throw new ArgumentException("The specified path does not exist or is not valid.", paramName);
        }
    }

    public static void AgainstInvalidVkToken(string vkAccessToken, string paramName)
    {
        var genericErrorMessage = "The token seems to be invalid.";

        if (string.IsNullOrEmpty(vkAccessToken))
        {
            throw new ArgumentException("Token cannot be null or empty.", paramName);
        }

        var isValidStart = vkAccessToken.StartsWith("vk1.") || vkAccessToken.StartsWith("vk2.");

        if (!isValidStart)
        {
            throw new ArgumentException(genericErrorMessage, paramName);
        }

        // Check if the token has a reasonable length
        if (vkAccessToken.Length < 85)
        {
            throw new ArgumentException(genericErrorMessage, paramName);
        }
    }
}
