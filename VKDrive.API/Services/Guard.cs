namespace VKDrive.API.Services;

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
}