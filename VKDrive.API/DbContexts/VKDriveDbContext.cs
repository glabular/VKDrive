using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using VKDrive.API.Models;

namespace VKDrive.API.DbContexts;

public class VKDriveDbContext : DbContext
{
    public VKDriveDbContext(DbContextOptions<VKDriveDbContext> options) : base(options)
    {
        
    }

    public DbSet<VkdriveEntry> VkdriveEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configures the 'Links' property in the VkdriveEntry entity to be stored as a comma-separated string in the database.
        // This is necessary because EF Core does not natively support storing collections (like List<string>) directly in a single column.
        // By converting the List<string> to a comma-separated string, we can store it in a single TEXT column.
        // When retrieving data from the database, the string is split back into a List<string> to restore the original format.
        modelBuilder.Entity<VkdriveEntry>()
            .Property(e => e.Links)
            .HasConversion(
                v => string.Join(',', v), // Convert List<string> to a comma-separated string
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() // Convert string back to List<string>
            );
    }
}