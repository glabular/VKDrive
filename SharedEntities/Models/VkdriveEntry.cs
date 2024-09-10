using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SharedEntities.Models;

public class VkdriveEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// The original name of processing file or folder
    /// </summary>
    public string OriginalName { get; set; } = string.Empty;

    public string OriginalPath { get; set; } = string.Empty;

    /// <summary>
    /// A unique name of the entry throughout the system.
    /// </summary>
    public string UniqueName { get; set; } = string.Empty;

    public string ArchivePassword { get; set; } = string.Empty;

    public byte[] AesKey { get; set; } = [];

    public byte[] AesIV { get; set; } = [];

    public DateTime CreationDate { get; set; }

    /// <summary>
    /// The size of the original file or folder, in bytes.
    /// </summary>
    public long Size { get; set; }

    public string Checksum { get; set; } = string.Empty;

    public bool IsFolder { get; set; }

    public List<string> Links { get; set; } = [];

    /// <summary>
    /// Converts the file size from bytes to a human-readable format.
    /// </summary>
    /// <returns>
    /// A string representing the file size in a readable format. The format will be in bytes, kilobytes (KB), megabytes (MB), or gigabytes (GB),
    /// depending on the size of the file. The returned string will use the largest appropriate unit to make the size easy to understand.
    /// </returns>
    public string GetReadableSize()
    {
        if (Size >= 1024 * 1024 * 1024)
        {
            return $"{Size / (1024 * 1024 * 1024):0.##} GB";
        }
        else if (Size >= 1024 * 1024)
        {
            return $"{Size / (1024 * 1024):0.##} MB";
        }
        else if (Size >= 1024)
        {
            return $"{Size / 1024:0.##} KB";
        }
        else
        {
            return $"{Size} bytes";
        }
    }
}
