using VKDrive.API.DbContexts;
using VKDrive.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using VKDrive.API.Services;
using SharedEntities.Models;

namespace VKDrive.API.Data;

/// <summary>
/// Provides a repository implementation for managing <see cref="VkdriveEntry"/> entities.
/// </summary>
public class VkdriveEntryRepository : IVkdriveEntryRepository
{
    private readonly VKDriveDbContext _context;
    private readonly ILogger<VkdriveEntryRepository> _logger;

    public VkdriveEntryRepository(VKDriveDbContext context, ILogger<VkdriveEntryRepository> logger)
    {
        Guard.AgainstNull(context, nameof(context));
        Guard.AgainstNull(logger, nameof(logger));

        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Asynchronously retrieves all <see cref="VkdriveEntry"/> entities from the database.
    /// </summary>
    public async Task<IEnumerable<VkdriveEntry>> GetAllEntriesAsync()
    {
        _logger.LogInformation("GetAllEntriesAsync method called.");

        try
        {
            var entries = await _context.VkdriveEntries
                .OrderBy(e => e.CreationDate)
                .ToListAsync();

            _logger.LogInformation("Successfully retrieved {Count} entries.", entries.Count);

            return entries;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving entries from the database.");
            throw;  // Re-throw the exception to be handled by the caller if needed
        }
    }

    public async Task<VkdriveEntry?> GetEntryByUniqueNameAsync(string uniqueName)
    {
        _logger.LogInformation("GetEntryByUniqueNameAsync method called with unique name: {UniqueName}", uniqueName);
        
        try
        {
            var entry = await _context.VkdriveEntries
                .SingleOrDefaultAsync(e => e.UniqueName == uniqueName);

            if (entry is null)
            {
                _logger.LogWarning("No entry found with unique name: {UniqueName}", uniqueName);

                return null;
            }
            
            _logger.LogInformation("Successfully retrieved entry with unique name: {UniqueName}", uniqueName);
            
            return entry;            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving entry with unique name: {UniqueName}", uniqueName);
            throw;  // Re-throw the exception to allow higher-level handling
        }
    }
        
    public async Task<VkdriveEntry?> GetEntryByIdAsync(int id)
    {
        _logger.LogInformation("GetEntryByIdAsync method called with id: {Id}", id);

        try
        {
            var entry = await _context.VkdriveEntries.FindAsync(id);

            if (entry is null)
            {
                _logger.LogWarning("No entry found with id: {Id}", id);
                return null;
            }
            
            _logger.LogInformation("Successfully retrieved entry with id: {Id}", id);            

            return entry;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving entry with id: {Id}", id);
            throw;  // Re-throw the exception to allow higher-level handling
        }
    }

    /// <summary>
    /// Asynchronously adds a new <see cref="VkdriveEntry"/> entity to the database and saves changes.
    /// </summary>
    /// <param name="entry">The <see cref="VkdriveEntry"/> entity to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="entry"/> is <c>null</c>.</exception>
    /// <exception cref="DbUpdateException">Thrown when an error occurs while saving changes to the database.</exception>
    public async Task AddEntryAsync(VkdriveEntry entry)
    {
        if (entry == null)
        {
            _logger.LogError("Attempted to add a null VkdriveEntry.");
            throw new ArgumentNullException(nameof(entry), "Entry cannot be null.");
        }

        _logger.LogInformation("Adding a new VkdriveEntry with UniqueName: {UniqueName}", entry.UniqueName);

        try
        {
            await _context.VkdriveEntries.AddAsync(entry);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully added VkdriveEntry with UniqueName: {UniqueName}", entry.UniqueName);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error occurred while saving VkdriveEntry with UniqueName: {UniqueName}", entry.UniqueName);
            throw;  // Re-throw the exception to allow higher-level handling
        }
    }

    //public async Task UpdateEntryAsync(VkdriveEntry entry)
    //{
    //    _context.VkdriveEntries.Update(entry);
    //    await _context.SaveChangesAsync();
    //}

    /// <summary>
    /// Asynchronously deletes a <see cref="VkdriveEntry"/> entity from the database by its unique name.
    /// </summary>
    /// <param name="uniqueName">The unique name of the entry to delete.</param>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="uniqueName"/> is <c>null</c> or empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when no entry with the specified <paramref name="uniqueName"/> is found.</exception>
    /// <exception cref="DbUpdateException">Thrown when an error occurs while saving changes to the database.</exception>

    public async Task DeleteEntryAsync(string uniqueName)
    {
        if (string.IsNullOrEmpty(uniqueName))
        {
            _logger.LogError("Attempted to delete an entry with a null or empty unique name.");
            throw new ArgumentException("Unique name cannot be null or empty.", nameof(uniqueName));
        }

        _logger.LogInformation("Attempting to delete VkdriveEntry with unique name: {UniqueName}", uniqueName);

        try
        {
            var entry = await GetEntryByUniqueNameAsync(uniqueName)
                ?? throw new KeyNotFoundException($"Entry with unique name '{uniqueName}' was not found.");

            _context.VkdriveEntries.Remove(entry);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted VkdriveEntry with unique name: {UniqueName}", uniqueName);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Delete operation failed. Entry with unique name '{UniqueName}' not found.", uniqueName);
            throw;  // Re-throw the exception to allow higher-level handling
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error occurred while deleting VkdriveEntry with unique name: {UniqueName}", uniqueName);
            throw;  // Re-throw the exception to allow higher-level handling
        }
    }

    public async Task<bool> IsEntryExistAsync(int id)
    {
        return await _context.VkdriveEntries.AllAsync(e => e.Id == id);
    }

    // ???
    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }
}