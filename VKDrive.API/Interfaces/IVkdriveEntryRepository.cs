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

    /// <summary>
    /// Asynchronously adds a new <see cref="VkdriveEntry"/> entity to the repository.
    /// </summary>
    /// <param name="entry">The <see cref="VkdriveEntry"/> entity to be added.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddEntryAsync(VkdriveEntry entry);

    /// <summary>
    /// Asynchronously deletes a <see cref="VkdriveEntry"/> entity from the repository by its unique name.
    /// </summary>
    /// <param name="uniqueName">The unique name of the <see cref="VkdriveEntry"/> entity to be deleted.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteEntryAsync(string uniqueName);
}