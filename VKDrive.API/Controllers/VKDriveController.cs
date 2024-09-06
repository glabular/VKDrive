using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VKDrive.API.Data;
using VKDrive.API.Interfaces;
using VKDrive.API.Models;
using VKDrive.API.Services;

namespace VKDrive.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VKDriveController : ControllerBase
{
    private readonly VkdriveEntryService _vkdriveEntryService;
    private readonly ILogger<VKDriveController> _logger;

    public VKDriveController(VkdriveEntryService vkdriveEntryService, ILogger<VKDriveController> logger)
    {
        Guard.AgainstNull(vkdriveEntryService, nameof(vkdriveEntryService));
        Guard.AgainstNull(logger, nameof(logger));

        _vkdriveEntryService = vkdriveEntryService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VkdriveEntry>>> GetAllEntriesAsync()
    {
        _logger.LogInformation($"{nameof(GetAllEntriesAsync)} method called.");

        try
        {
            var entities = await _vkdriveEntryService.GetAllEntriesAsync();
            _logger.LogInformation("Successfully retrieved all entries.");

            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving entries.");

            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("download-entry/{uniqueName}")]
    public async Task<ActionResult<string>> DownloadEntryAsync(string uniqueName)
    {
        if (string.IsNullOrWhiteSpace(uniqueName))
        {
            _logger.LogWarning("DownloadEntryAsync failed due to empty unique name.");

            return BadRequest("Unique name cannot be null or whitespace.");
        }

        _logger.LogInformation("DownloadEntryAsync method called with uniqueName: {UniqueName}", uniqueName);

        try
        {
            var originalEntryPath = await _vkdriveEntryService.GetOriginalFileAsync(uniqueName);

            if (System.IO.File.Exists(originalEntryPath))
            {
                _logger.LogInformation("Downloaded file saved at path: {OriginalEntryPath}", originalEntryPath);

                return Ok(originalEntryPath);
            }
            else if (Directory.Exists(originalEntryPath))
            {
                _logger.LogInformation("Downloaded folder saved at path: {OriginalEntryPath}", originalEntryPath);

                return Ok(originalEntryPath);
            }
            else
            {
                _logger.LogWarning("Unknown error occured while retrieving entry with the unique name: {UniqueName}", uniqueName);

                return NotFound($"Unknown error occured while retrieving entry with the unique name \"{uniqueName}\".");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while downloading entry with uniqueName: {UniqueName}", uniqueName);
            
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpPost("upload-entry")]
    public async Task<IActionResult> UploadEntryAsync([FromBody] string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            _logger.LogWarning("UploadEntryAsync failed due to empty file path.");

            return BadRequest("File path cannot be null or whitespace.");
        }

        _logger.LogInformation("UploadEntryAsync method called with path: {Path}", path);

        if (!System.IO.File.Exists(path) && !System.IO.Directory.Exists(path))
        {
            _logger.LogWarning("UploadEntryAsync failed because file or directory does not exist at path: {Path}", path);

            return NotFound("Failed to process because file or directory does not exist.");
        }

        try
        {
            await _vkdriveEntryService.CreateAndSaveEntryAsync(path);
            var isFolder = Directory.Exists(path);
            var name = isFolder ? System.IO.Path.GetFileName(Path.TrimEndingDirectorySeparator(path)) : System.IO.Path.GetFileName(path);
            
            _logger.LogInformation("Successfully processed and saved {Name} at path: {Path}", name, path);

            return Ok($"{name} processed and saved successfully.");
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid argument error while uploading entry with path: {Path}", path);

            return BadRequest($"Invalid argument: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while uploading entry with path: {Path}", path);

            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }

    [HttpDelete("delete-entry/{uniqueName}")]
    public async Task<ActionResult> DeleteEntryAsync(string uniqueName)
    {
        if (string.IsNullOrWhiteSpace(uniqueName))
        {
            _logger.LogWarning("DeleteEntryAsync failed due to empty unique name.");

            return BadRequest("Unique name cannot be null or whitespace.");
        }

        _logger.LogInformation("DeleteEntryAsync method called with uniqueName: {UniqueName}", uniqueName);

        try
        {
            await _vkdriveEntryService.DeleteEntryAsync(uniqueName);

            _logger.LogInformation("Successfully deleted entry with uniqueName: {UniqueName}", uniqueName);

            // Return a NoContent result to indicate the resource was successfully deleted
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Argument exception occurred while deleting entry with uniqueName: {UniqueName}", uniqueName);

            // Return a BadRequest if the uniqueName is null or empty
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "Entry not found while deleting entry with uniqueName: {UniqueName}", uniqueName);

            // Return a NotFound result if the entry was not found in the database
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting entry with uniqueName: {UniqueName}", uniqueName);

            // Return a generic error response for any other exceptions
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
