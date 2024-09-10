using Microsoft.EntityFrameworkCore;
using VKDrive.API.Data;
using VKDrive.API.DbContexts;
using VKDrive.API.Interfaces;
using VKDrive.API.Services;
using Microsoft.Extensions.Logging;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureLogger(builder);
        ConfigureServices(builder.Services);

        builder.Services.AddDbContext<VKDriveDbContext>(
            dbContextOptions => dbContextOptions.UseSqlite(builder.Configuration["ConnectionStrings:VKDriveDbConnectionString"]));
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }

    private static void ConfigureLogger(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IVkdriveEntryRepository, VkdriveEntryRepository>();
        services.AddScoped<IEncryptionService, AesEncryptionService>();
        services.AddScoped<IHashingService, Sha256HashingService>();
        services.AddScoped<IArchiveService, ZipService>();
        services.AddScoped<IVkApiService, VkApiService>();
        services.AddScoped<VkdriveEntryService>();
        services.AddTransient<FilePartitionerService>();
    }
}