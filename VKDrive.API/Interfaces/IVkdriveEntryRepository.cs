using SharedEntities.Models;

namespace VKDrive.API.Interfaces;

/// <summary>
/// Defines the contract for a repository that manages <see cref="VkdriveEntry"/> entities.
/// </summary>
public interface IVkdriveEntryRepository
{
    /// <summary>
    /// Asynchronously retrieves all <see cref="VkdriveEntry"/> entities from the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{VkdriveEntry}"/>.</returns>
    Task<IEnumerable<VkdriveEntry>> GetAllEntriesAsync();

    /// <summary>
    /// Asynchronously retrieves a <see cref="VkdriveEntry"/> entity by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the entry to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="VkdriveEntry"/> entity if found; otherwise, <c>null</c>.</returns>
    Task<VkdriveEntry?> GetEntryByIdAsync(int id);

    /// <summary>
    /// Asynchronously retrieves a <see cref="VkdriveEntry"/> entity by its unique name.
    /// </summary>
    /// <param name="uniqueName">The unique name of the entry to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="VkdriveEntry"/> entity if found; otherwise, <c>null</c>.</returns>
    Task<VkdriveEntry?> GetEntryByUniqueNameAsync(string uniqueName);

    Task AddEntryAsync(VkdriveEntry entry);

    //Task UpdateEntryAsync(VkdriveEntry entry);

    Task DeleteEntryAsync(string uniqueName);

    Task<bool> IsEntryExistAsync(int id);

    Task<bool> SaveChangesAsync();
}
