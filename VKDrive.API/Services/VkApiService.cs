using SharedEntities;
using SharedEntities.Settings;
using System.Text.Json.Nodes;
using VKDrive.API.Interfaces;

namespace VKDrive.API.Services;

public class VkApiService : IVkApiService
{
    private readonly string _accessTokenVkAPI;
    private readonly string _apiVersion;
    private readonly int _groupId;
    private readonly int _httpClientTimeoutSeconds;
    private readonly ILogger<VkApiService> _logger;

    public VkApiService(ILogger<VkApiService> logger)
    {
        Guard.AgainstNull(logger, nameof(logger));
        _logger = logger;

        var settings = SettingsManager.LoadSettings();
        
        _accessTokenVkAPI = settings.VkAccessToken;
        _apiVersion = settings.ApiVersion;

        if (settings.GroupID <= 0)
        {
            throw new ArgumentException($"{nameof(settings.GroupID)} cannot be zero or negative.", nameof(settings.GroupID));
        }

        _groupId = settings.GroupID;

        _httpClientTimeoutSeconds = settings.HttpClientTimeout;
    }

    /// <summary>
    /// Uploads a file to the VK server, saves it, and retrieves the download URL.
    /// </summary>
    /// <param name="filePath">The local path of the file to be uploaded.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the URL to download the uploaded file.</returns>
    /// <remarks>
    /// This method first retrieves an upload URL from the VK API, then uploads the file to that URL, 
    /// and finally saves the file on the VK server to get the final download link.
    /// </remarks>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request to the VK API fails.</exception>
    public async Task<string> UploadFileToVkServer(string filePath)
    {
        Guard.AgainstNullOrWhitespace(filePath, nameof(filePath));
        Guard.AgainstInvalidPath(filePath, nameof(filePath));

        var currentUrl = await GetUploadURLAsync();
        var uploadedFileInfo = await UploadFileAsync(filePath, currentUrl);
        var savedFileInfo = await SaveFileOnServerAsync(uploadedFileInfo);

        // TODO: Captcha check here!!!        

        var json = JsonObject.Parse(savedFileInfo);

        // The way to get the link according to VK API.
        var urlToDownloadFile = json["response"]["doc"]["url"].ToString();

        return urlToDownloadFile;
    }

    public async Task<string> DeleteDocumentFromVkServer(string docUrl)
    {
        Guard.AgainstNullOrWhitespace(docUrl, nameof(docUrl));

        using var client = new HttpClient();
        // NB! HttpMethod.Delete returns 418 I'm a teapot status code
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.vk.com/method/docs.delete");
        var ownerID = ExtractOwnerID(docUrl);
        var docID = ExtractDocID(docUrl);

        using var content = new MultipartFormDataContent
        {
            { new StringContent(ownerID), "owner_id" },
            { new StringContent(docID), "doc_id" },
            { new StringContent(_accessTokenVkAPI), "access_token" },
            { new StringContent("5.131"), "v" }
        };
        request.Content = content;

        var response = await client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Successfully deleted document with URL: {DocUrl}.", docUrl);

            return responseBody;
        }
        else
        {
            // Log the response status code and reason phrase for debugging
            var statusCode = response.StatusCode;
            var reasonPhrase = response.ReasonPhrase;
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogError("Failed to delete document with URL: {DocUrl}. Status code: {StatusCode}. Reason phrase: {ReasonPhrase}. Response content: {ResponseContent}",
                docUrl, statusCode, reasonPhrase, responseBody);

            // Throw an exception with the response details for further handling
            throw new HttpRequestException($"Request failed with status code {statusCode}. Reason: {reasonPhrase}. Content: {responseContent}");
        }
    }

    /// <summary>
    /// Retrieves the URL for uploading a file to the VK server.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the upload URL as a string.</returns>
    /// <remarks>
    /// This method sends a request to the VK API to obtain an upload URL, which is used in subsequent file upload operations.
    /// If the request fails, it retries up to 5 times before giving up.
    /// </remarks>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request to the VK API fails after multiple attempts.</exception>
    private async Task<string> GetUploadURLAsync()
    {
        var maxAttempts = 5;
        var url = string.Empty;

        for (int attempt = 1; attempt < maxAttempts; attempt++)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(
                    HttpMethod.Post, 
                    "https://api.vk.com/method/docs.getWallUploadServer");

                var content = new MultipartFormDataContent
                {
                    { new StringContent(_accessTokenVkAPI), "access_token" },
                    { new StringContent(_groupId.ToString()), "group_id" },
                    { new StringContent(_apiVersion), "v" }
                };

                request.Content = content;

                _logger.LogInformation("Attempt {Attempt} to get upload URL.", attempt);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var json = JsonObject.Parse(responseBody);
                url = json["response"]["upload_url"].ToString();

                _logger.LogInformation("Successfully retrieved upload URL on the {Attempt} attempt.", attempt);
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during getting the URL attempt {Attempt}.", attempt);

                if (attempt == maxAttempts)
                {
                    _logger.LogError("Failed to get URL after multiple attempts.");
                    throw;
                }

                await Task.Delay(1000);
            }
        }

        return url;
    }

    /// <summary>
    /// Uploads a file to the specified VK server URL.
    /// </summary>
    /// <param name="file">The local path of the file to be uploaded.</param>
    /// <param name="URL">The VK server URL to which the file will be uploaded.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the server's response as a string.</returns>
    /// <remarks>
    /// This method attempts to upload a file to the VK server. It retries up to 15 times if the upload fails.
    /// </remarks>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request to upload the file fails after multiple attempts.</exception>
    private async Task<string> UploadFileAsync(string file, string URL)
    {
        var maxAttempts = 10;
        var responseBody = string.Empty;

        for (int attempt = 1; attempt < maxAttempts; attempt++)
        {
            try
            {
                using var client = new HttpClient();
                using var request = new HttpRequestMessage(HttpMethod.Post, URL);

                client.Timeout = TimeSpan.FromSeconds(_httpClientTimeoutSeconds);
                var fileName = Path.GetFileName(file);
                var fileContent = File.ReadAllBytes(file);
                var content = new MultipartFormDataContent
                {
                    {
                        new ByteArrayContent(fileContent), "file", fileName
                    }
                };
                request.Content = content;

                _logger.LogInformation("Attempt {Attempt} to upload file.", attempt);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("File uploaded to the server successfully.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during upload attempt {Attempt}.", attempt);
                if (attempt == maxAttempts)
                {
                    _logger.LogError("Failed to upload file after multiple attempts.");
                    throw;
                }

                await Task.Delay(1000);
            }
        }

        return responseBody;
    }

    /// <summary>
    /// Saves the uploaded file information on the VK server and returns the server's response.
    /// </summary>
    /// <param name="uploadedFileInfo">The information about the uploaded file returned by the VK server.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the server's response as a string.</returns>
    /// <remarks>
    /// This method sends a request to the VK API to save the uploaded file. 
    /// The VK API responds with information about the saved file, which is returned as a string.
    /// </remarks>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request to save the file on the VK server fails.</exception>
    /// <exception cref="JsonReaderException">Thrown if the response does not contain valid JSON.</exception>
    private async Task<string> SaveFileOnServerAsync(string uploadedFileInfo)
    {
        try
        {
            var json = JsonObject.Parse(uploadedFileInfo); // System.Text.Json.JsonReaderException: 'The input does not contain any JSON tokens. 
            // TODO: handle null possibility.
            var fileInfo = json!["file"].ToString(); // VK server always returns valid json with the "file" node

            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.vk.com/method/docs.save");
            using var content = new MultipartFormDataContent
            {
                { new StringContent(_accessTokenVkAPI), "access_token" },
                { new StringContent(fileInfo), "file" },
                { new StringContent(_apiVersion), "v" }
            };
            request.Content = content;

            _logger.LogInformation("Saving file information on the server.");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("File information saved on the server successfully.");

            return responseBody;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving file on the server.");
            throw;
        }
    }

    private static string ExtractDocID(string link)
    {
        var docID = string.Empty;
        var startIndex = link.IndexOf('_');

        if (startIndex != -1)
        {
            var endIndex = link.IndexOf('?');

            if (endIndex != -1)
            {
                var substring = link.Substring(startIndex + 1, endIndex - startIndex - 1);
                docID = substring.Replace("_", "");
            }
        }

        return docID;
    }

    private static string ExtractOwnerID(string link)
    {
        var ownerID = string.Empty;
        var startIndex = link.IndexOf('-');

        if (startIndex != -1)
        {
            var endIndex = link.IndexOf('_');

            if (endIndex != -1)
            {
                ownerID = link[startIndex..endIndex];
            }
        }

        return ownerID;
    }
}