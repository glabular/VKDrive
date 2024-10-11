using SharedEntities.Models;
using System.Diagnostics;

namespace SharedEntities.Settings;

public class Settings
{
    private int _chunkToUploadSizeInMegabytes = 190;

    public int ChunkToUploadSizeInMegabytes
    {
        get => _chunkToUploadSizeInMegabytes;
        set
        {
            _chunkToUploadSizeInMegabytes = Math.Min(value, 200);
        }
    }

    public string VkAccessToken { get; set; } = string.Empty;

    public int HttpClientTimeout { get; set; } = 86400;

    public ProcessPriorityClass ProcessPriority { get; set; } = ProcessPriorityClass.Normal;

    public int ArchivePasswordLength { get; set; } = 5;

    public int GroupID { get; set; } = 39530977;

    public string ApiVersion { get; set; } = "5.131";

    public bool SoundsOn { get; set; }

    public bool AskBeforeDelete { get; set; } = true;

    public bool OpenFolderAfterDownload { get; set; } = true;

    public MyCompressionLevel CompressionLevel { get; set; } = MyCompressionLevel.Best;
}
