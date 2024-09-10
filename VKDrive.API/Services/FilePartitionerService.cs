using System.Text.RegularExpressions;

namespace VKDrive.API.Services;

/// <summary>
/// A class for partitioning large files into smaller segments 
/// and reassembling those segments back into the original file.
/// </summary>
public class FilePartitionerService
{ 
    /// <summary>
    /// Splits a file into multiple smaller parts of specified size and saves them in the output directory.
    /// </summary>
    /// <param name="fileToSplit">The path of the file to be split.</param>
    /// <param name="chunkSizeMB">The maximum size of each chunk in megabytes (MB).</param>
    /// <param name="outputDirectory">The directory where the split file parts will be saved.</param>
    /// <param name="outputFileName">The base name for the output file parts. Each part will have a sequential suffix appended.</param>
    /// <returns>A list of file paths representing the split parts of the original file.</returns>
    /// <remarks>
    /// This method reads the specified file, splits it into smaller parts of up to the specified chunk size, 
    /// and writes each part to the output directory with a sequentially numbered file name.
    /// If the file is smaller than the specified chunk size, it is copied directly without splitting.
    /// </remarks>
    public List<string> SplitFile(string fileToSplit, int chunkSizeMB, string outputDirectory, string outputFileName)
    {
        ValidateFileSplittingInput(fileToSplit, chunkSizeMB, outputDirectory, outputFileName);

        var chunkSize = CalculateChunkSize(fileToSplit, chunkSizeMB);
        var fileParts = new List<string>();
        var bufferSize = 1024 * 1024 * chunkSize;
        var index = 1;
        using var input = new FileStream(fileToSplit, FileMode.Open, FileAccess.Read);

        Directory.CreateDirectory(outputDirectory);

        if (input.Length <= bufferSize)
        {
            var outputFile = Path.Combine(outputDirectory, $"{outputFileName}.vkd");
            File.Copy(fileToSplit, outputFile);
            fileParts.Add(outputFile);

            return fileParts;
        }

        var buffer = new byte[bufferSize];

        while (input.Position < input.Length)
        {
            var outputFile = Path.Combine(outputDirectory, $"{outputFileName}-{index}.vkd");
            using var output = new FileStream(outputFile, FileMode.Create);
            var remaining = bufferSize;

            while (remaining > 0 && input.Position < input.Length)
            {
                var bytesRead = input.Read(buffer, 0, Math.Min(remaining, bufferSize));
                output.Write(buffer, 0, bytesRead);
                remaining -= bytesRead;
            }

            fileParts.Add(outputFile);
            index++;
        }

        return fileParts;
    }

    /// <summary>
    /// Assembles multiple file parts into a single original file.
    /// </summary>
    /// <param name="filePartsPaths">A list of file paths representing the parts to be joined.</param>
    /// <param name="assembledFilePath">The path where the assembled file will be saved.</param>
    /// <remarks>
    /// This method takes a list of file parts and sequentially combines them into a single output file.
    /// The file parts should be in the correct order in the list for the resulting file to be assembled correctly.
    /// </remarks>
    public void JoinParts(IEnumerable<string> filePartsPaths, string assembledFilePath)
    {
        ValidateFileJoiningInput(filePartsPaths, assembledFilePath);

        using var outfile = new FileStream(assembledFilePath, FileMode.Create);
        foreach (var filePartPath in filePartsPaths)
        {
            using var infile = new FileStream(filePartPath, FileMode.Open);
            infile.CopyTo(outfile);
        }
    }

    private static void ValidateFileJoiningInput(IEnumerable<string> filePartsPaths, string assembledFilePath)
    {
        Guard.AgainstNullOrEmptyCollection(filePartsPaths, nameof(filePartsPaths));
        Guard.AgainstNullOrWhitespace(assembledFilePath, nameof(assembledFilePath));
    }

    private static void ValidateFileSplittingInput(string fileToSplit, int chunkSizeMB, string outputDirectory, string outputFileName)
    {
        Guard.AgainstInvalidPath(fileToSplit, nameof(fileToSplit));
        Guard.AgainstNullOrWhitespace(outputFileName, nameof(outputFileName));
        Guard.AgainstNullOrWhitespace(outputDirectory, nameof(outputDirectory));

        if (chunkSizeMB < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(chunkSizeMB), chunkSizeMB, "Chunk size must be 1 MB or more.");
        }
    }

    /// <summary>
    /// Calculates the optimal chunk size in megabytes (MB) for splitting a file into smaller parts.
    /// </summary>
    /// <param name="file">The path of the file to be split.</param>
    /// <param name="maxChunkSizeMb">The maximum desired size of each chunk in megabytes (MB).</param>
    /// <returns>The optimal chunk size in megabytes (MB) to ensure the file is split evenly.</returns>
    /// <remarks>
    /// This method determines the chunk size based on the file's total size and the maximum chunk size specified.
    /// The result is adjusted to ensure that the file is divided into even chunks.
    /// </remarks>
    private static int CalculateChunkSize(string file, int maxChunkSizeMb)
    {
        // TODO: Refactor the method for it to WORK!
        var fileSizeBytes = new FileInfo(file).Length;
        var maxChunkSizeBytes = maxChunkSizeMb * 1024 * 1024;
        var numChunks = (int)Math.Ceiling((double)fileSizeBytes / maxChunkSizeBytes);
        var chunkSizeBytes = (int)Math.Ceiling((double)fileSizeBytes / numChunks);

        return (chunkSizeBytes / 1024 / 1024) + 1;
    }
}
