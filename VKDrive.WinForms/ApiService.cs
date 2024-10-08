﻿using SharedEntities.Models;
using System.Text;
using System.Text.Json;

namespace VKDrive.WinForms;

internal class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7288/"),
            Timeout = TimeSpan.FromSeconds(86400)
        };        

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<string> AddEntryAsync(string path)
    {
        var content = new StringContent(JsonSerializer.Serialize(path), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/VKDrive/upload-entry", content);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        return $"Error: {response.ReasonPhrase}";
    }

    public async Task<string?> DownloadEntryAsync(string uniqueName)
    {


        try
        {
            var response = await _httpClient.GetAsync($"api/vkdrive/download-entry/{uniqueName}");

            if (response.IsSuccessStatusCode)
            {
                var originalEntryPath = await response.Content.ReadAsStringAsync();

                return originalEntryPath;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"No file or directory found with the unique name \"{uniqueName}\".");

                return null;
            }
            else
            {
                Console.WriteLine($"Unexpected response status code: {response.StatusCode}");

                return null;
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");

            return null;
        }
    }

    public async Task<List<VkdriveEntry>?> GetEntriesAsync()
    {
        var response = await _httpClient.GetAsync("api/VKDrive");

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<VkdriveEntry>>(jsonResponse, _jsonSerializerOptions);
        }

        return null;
    }

    public async Task<bool> DeleteEntryAsync(string uniqueName)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/VKDrive/delete-entry/{uniqueName}");

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"Failed to delete the entry: {response.ReasonPhrase}");

                return false;
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");

            return false;
        }
    }

    public async Task<bool> IsApiAvailableAsync(int maxRetries = 3, int delayMilliseconds = 1000)
    {
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/health");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch
            {
                // Log the exception if needed
            }

            // Wait before retrying
            await Task.Delay(delayMilliseconds);
            delayMilliseconds += 500;
        }

        return false;
    }
}
