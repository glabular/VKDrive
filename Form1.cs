using Ionic.Zip;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Media;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using VKDrive;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private readonly string statusbarLabelDefaultText = "Готово";
        private readonly string VKDriveFolder = @"C:\VKDrive"; // TODO
        private readonly string temporaryFolder = @"C:\VKDrive\tmp"; // TODO
        private readonly string downloadedFolder = @"C:\VKDrive\Downloaded";// TODO
        private string _jsonSettingsLocation;
        private string _jsonCloudFilesLocation;
        private List<CloudFile> _cloudFiles;
        private Settings settings;
        private CloudFile _selectedFile;
        private int _easterEggClicksCounter;
        //[DllImport("kernel32.dll")] // Enable console
        //static extern bool AllocConsole();// Enable console

        public Form1()
        {
            //AllocConsole(); // Enable console
            EnsureSystemFoldersExist();
            var folderName = "VKDrive";
            var userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var appFolderPath = Path.Combine(userFolderPath, folderName);
            _jsonSettingsLocation = Path.Combine(appFolderPath, "VKDrive_settings.json");
            _jsonCloudFilesLocation = Path.Combine(appFolderPath, "Files_list.json");

            if (!Directory.Exists(appFolderPath))
            {
                Directory.CreateDirectory(appFolderPath);
            }

            if (File.Exists(_jsonSettingsLocation))
            {
                var jsonFile = File.ReadAllText(_jsonSettingsLocation);
                settings = System.Text.Json.JsonSerializer.Deserialize<Settings>(jsonFile);
            }
            else
            {
                settings = new Settings { };

                var jsonOptions = new JsonSerializerOptions()
                {
                    WriteIndented = true,
                };
                var jsonString = System.Text.Json.JsonSerializer.Serialize(settings, jsonOptions);
                File.WriteAllText(_jsonSettingsLocation, jsonString);
            }

            _cloudFiles = new List<CloudFile>();

            if (File.Exists(_jsonCloudFilesLocation))
            {
                var jsonString = File.ReadAllText(_jsonCloudFilesLocation);
                if (jsonString != "[]")
                {
                    _cloudFiles = System.Text.Json.JsonSerializer.Deserialize<List<CloudFile>>(jsonString);
                }
            }

            SetPriority();

            InitializeComponent();
            listBox1.DataSource = _cloudFiles;
            listBox1.DisplayMember = "NameAndSize";

            MaximizeBox = false;

            notifyIcon1.BalloonTipTitle = "VKDrive";
            notifyIcon1.BalloonTipText = "VKDrive";
            notifyIcon1.Text = "VKDrive";

            listBox1.AllowDrop = true;

            RefreshFileList();
        }

        private async Task UploadFileToVKDriveAsync(string file_path)
        {
            StartProgressBar();
            toolStripStatusLabel1.Text = "Загрузка файла в облако";
            DisableButtons();

            var cloudFile = new CloudFile();

            var originalFileName = Path.GetFileName(file_path);
            cloudFile.Name = originalFileName;

            var fileSize = string.Empty;
            await Task.Run(() =>
            {
                fileSize = GetFileSize(file_path);
            });

            cloudFile.Size = fileSize;
            toolStripStatusLabel1.Text = "Вычисление хэш-суммы";
            var sha256Checksum = string.Empty;
            await Task.Run(() =>
            {
                sha256Checksum = GetSHA256Checksum(file_path).ToLower();
            });

            var systemFolder = CreateSystemFolder(sha256Checksum);

            toolStripStatusLabel1.Text = "Архивирование файла";
            var uniqueName = $"{Guid.NewGuid()}";
            cloudFile.UniqueName = uniqueName;
            var outputArchive = Path.Combine(temporaryFolder, $"{uniqueName}.7z");

            var archivePassword = string.Empty;
            await Task.Run(() =>
            {
                archivePassword = GeneratePassword(settings.ArchivePasswordLength);
            });

            await Task.Run(() =>
            {
                CompressFile(file_path, outputArchive, archivePassword);
            });

            toolStripStatusLabel1.Text = "Шифрование файла";
            var key = GenerateEncryptionKey(settings.AesPasswordLength);
            var initVector = GenerateEncryptionKey(16);
            var encryptedFilePath = string.Empty;
            await Task.Run(() =>
            {
                encryptedFilePath = EncryptFile(outputArchive, key, initVector);
            });

            toolStripStatusLabel1.Text = "Разделение файла на части";
            var chunkSize = CalculateChunkSize(encryptedFilePath, settings.ChunkToUploadSize);

            await Task.Run(() =>
            {
                SplitFile(encryptedFilePath, chunkSize, sha256Checksum, uniqueName);
            });

            if (File.Exists(outputArchive))
            {
                toolStripStatusLabel1.Text = "Удаление архива";
                File.Delete(outputArchive);
            }

            File.Delete(encryptedFilePath);

            var filesPaths = Directory.GetFiles(systemFolder);
            filesPaths = SortFiles(filesPaths);
            var links = new List<string>();

            var uploadingStatus = "Загрузка файлов на сервер ВК";
            toolStripStatusLabel1.Text = uploadingStatus;
            var uploadingFileCounter = 0;

            foreach (var file in filesPaths)
            {
                uploadingFileCounter++;
                toolStripStatusLabel1.Text = $"{uploadingStatus}: {uploadingFileCounter}/{filesPaths.Length}";
                var currentURL = await GetUploadURLAsync();
                var uploadedFileInfo = await UploadFileAsync(file, currentURL.ToString());
                var savedFileInfo = await SaveFileOnServer(uploadedFileInfo);
                if (uploadingFileCounter % 5 == 0)
                {
                    var rnd = new Random();
                    toolStripStatusLabel1.Text = "VK API rest, please wait...";
                    await Task.Delay(rnd.Next(1000 * 60 * 4, 1000 * 60 * 8)); // Задержка, чтобы "успокоить" VK API...
                }

                if (IsCaptchaNeeded(savedFileInfo))
                {
                    var captchaSid = GetCaptchaSID(savedFileInfo);
                    var captchaImgUrl = GetCaptchaImgUrl(savedFileInfo);
                    SystemSounds.Exclamation.Play();
                    ShowCaptchaWindow(uploadedFileInfo, captchaSid, captchaImgUrl, links);

                    continue;
                }
                else
                {
                    var json = JsonObject.Parse(savedFileInfo);
                    var URLToDownloadFile = json["response"]["doc"]["url"].ToString();
                    links.Add(URLToDownloadFile);
                }
            }

            var jsonPath = Path.Combine(VKDriveFolder, $"{sha256Checksum}.json");
            CreateJsonFile(originalFileName, fileSize, key, initVector, archivePassword, links, jsonPath, uniqueName);

            cloudFile.jsonPath = jsonPath;
            cloudFile.CreationDate = DateTime.Now;
            _cloudFiles.Add(cloudFile);

            var jsonString = System.Text.Json.JsonSerializer.Serialize(_cloudFiles, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(_jsonCloudFilesLocation, jsonString);

            var deletionStatus = "Удаление временных файлов";
            toolStripStatusLabel1.Text = deletionStatus;
            var di = new DirectoryInfo(systemFolder);
            var tempFilesCounter = 1;
            var tempFiles = di.GetFiles();

            foreach (var file in tempFiles)
            {
                toolStripStatusLabel1.Text = $"{deletionStatus}: {tempFilesCounter}/{tempFiles.Length}";
                file.Delete();
                tempFilesCounter++;
            }

            if (settings.SoundsOn)
            {
                var audio = new SoundPlayer(VKDrive.Properties.Resources.Speech_On);
                audio.Play();
            }

            // NB! Данный (UploadFileToVkAsync) метод не вызывает остановку прогресс бара в конце выполнения работы, т.к.
            // метод RefreshFileList вызывает остановку прогресс бара в конце выполнения своей работы.

            await RefreshFileList();
        }

        private void ShowCaptchaWindow(string requestToRepeat, string captchaSid, string captchaImgUrl, List<string> links)
        {
            //Создать окно
            var captchaForm = new Form
            {
                Width = 300,
                Height = 250,
                Text = "Ведите Captcha",
                MinimizeBox = false,
                StartPosition = FormStartPosition.CenterScreen
            };

            var captchaPictureBox = new PictureBox
            {
                Width = 200,
                Height = 100,
                Location = new Point(50, 20),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            captchaPictureBox.Load(captchaImgUrl);
            captchaForm.Controls.Add(captchaPictureBox);

            var captchaTextBox = new TextBox
            {
                Width = 200,
                Location = new Point(50, 130)
            };
            captchaForm.Controls.Add(captchaTextBox);

            var submitButton = new Button
            {
                Text = "Отправить",
                Location = new Point(100, 160)
            };
            captchaForm.Controls.Add(submitButton);

            string captchaInput = string.Empty;
            submitButton.Click += async (sender, e) =>
            {
                captchaInput = captchaTextBox.Text;

                var json = JsonObject.Parse(requestToRepeat);
                var fileInfo = json["file"].ToString();

                using var client = new HttpClient();
                using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.vk.com/method/docs.save");
                using var content = new MultipartFormDataContent
                {
                    { new StringContent(settings.AccessToken), "access_token" },
                    { new StringContent(captchaSid), "captcha_sid" },
                    { new StringContent(captchaInput), "captcha_key" },
                    { new StringContent(fileInfo), "file" },
                    { new StringContent(settings.ApiVersion), "v" }
                };
                request.Content = content;

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var jsonObj = JsonObject.Parse(responseBody);
                var URLToDownloadFile = jsonObj["response"]["doc"]["url"].ToString(); // TODO Если капча введена неправильно то System.NullReferenceException.
                links.Add(URLToDownloadFile);

                captchaForm.Close();
            };

            captchaForm.ShowDialog();
        }

        private bool IsCaptchaNeeded(string jsonResponse)
        {
            dynamic responseObject = JsonConvert.DeserializeObject(jsonResponse);

            if (responseObject.error != null && responseObject.error.error_code == 14)
            {
                return true;
            }

            return false;
        }

        private async Task RefreshFileList()
        {
            StartProgressBar();
            DisableButtons();
            toolStripStatusLabel1.Text = "Обновление списка";
            listBox1.DataSource = null;
            if (settings.SortByName)
            {
                _cloudFiles = _cloudFiles.OrderBy(file => file.Name).ToList();
            }
            if (settings.SortByDate)
            {
                _cloudFiles = _cloudFiles.OrderBy(file => file.CreationDate).Reverse().ToList();
            }

            listBox1.DataSource = _cloudFiles;
            listBox1.DisplayMember = "NameAndSize";
            listBox1.Refresh();
            await Task.Delay(1);
            EnableButtons();
            StopProgressBar();
            toolStripStatusLabel1.Text = statusbarLabelDefaultText;
        }

        public static string GetCaptchaSID(string json)
        {
            var jsonObject = JObject.Parse(json);

            return jsonObject["error"]["captcha_sid"].ToString();
        }

        public static string GetCaptchaImgUrl(string json)
        {
            var jsonObject = JObject.Parse(json);

            return jsonObject["error"]["captcha_img"].ToString();
        }

        private void DisableButtons()
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button_download.Enabled = false;
            button_deleteFromCloud.Enabled = false;
            listBox1.Enabled = false;
            settingsButton.Enabled = false;
        }

        private void EnableButtons()
        {
            button1.Enabled = true;
            button2.Enabled = true;
            button_download.Enabled = true;
            button_deleteFromCloud.Enabled = true;
            listBox1.Enabled = true;
            settingsButton.Enabled = true;
        }

        private void EnsureSystemFoldersExist()
        {
            if (!Directory.Exists(temporaryFolder))
            {
                Directory.CreateDirectory(temporaryFolder);
            }

            if (!Directory.Exists(downloadedFolder))
            {
                Directory.CreateDirectory(downloadedFolder);
            }
        }

        private string ByteToString(byte[] array)
        {
            var sb = new StringBuilder();

            foreach (var c in array)
            {
                sb.Append(c);
                sb.Append('\n');
            }

            return sb.ToString();
        }

        private byte[] StringToByte(string str)
        {
            var byteList = new List<byte>();
            var stringArr = str.Split('\n');

            foreach (var s in stringArr)
            {
                byte b;

                if (byte.TryParse(s, out b))
                {
                    byteList.Add(b);
                }
            }

            return byteList.ToArray();
        }        

        private List<string> GetLinksFromJson(string jsonFile)
        {
            var jsonText = File.ReadAllText(jsonFile);
            var jsonData = JObject.Parse(jsonText);

            return jsonData["Links"].ToObject<List<string>>();
        }

        private string[] GetJsonFiles()
        {
            return Directory.GetFiles(VKDriveFolder, "*.json");
        }

        public static string GeneratePassword(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-_=+[{]}|;:',<.>/?`~";
            var password = new char[length];
            var rng = RandomNumberGenerator.Create();
            var buffer = new byte[sizeof(uint)];

            for (int i = 0; i < length; i++)
            {
                rng.GetBytes(buffer);
                uint randomIndex = BitConverter.ToUInt32(buffer, 0) % (uint)chars.Length;
                password[i] = chars[(int)randomIndex];
            }

            return new string(password);
        }        

        private async Task<string> SaveFileOnServer(string uploadedFileInfo)
        {
            var json = JsonObject.Parse(uploadedFileInfo);
            var fileInfo = json["file"].ToString();

            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.vk.com/method/docs.save");
            using var content = new MultipartFormDataContent
            {
                { new StringContent(settings.AccessToken), "access_token" },
                { new StringContent(fileInfo), "file" },
                { new StringContent("5.131"), "v" }
            };
            request.Content = content;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        private async Task<string> UploadFileAsync(string file, string URL)
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(settings.HttpClientTimeout);
            using var request = new HttpRequestMessage(HttpMethod.Post, URL);
            var fileName = Path.GetFileName(file);
            var fileContent = File.ReadAllBytes(file);
            var content = new MultipartFormDataContent
            {
                {
                    new ByteArrayContent(fileContent), "file", fileName
                }
            };
            request.Content = content;

            var response = await client.SendAsync(request);
            var responseBody = string.Empty;
            try
            {
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();

            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    // Handle the gateway timeout error
                    ShowPopupErrorMessagebox("Ошибка", "The server did not respond within the expected time. Please try again later.");
                }
                else
                {
                    // Handle any other HTTP request error
                    ShowPopupErrorMessagebox("Ошибка", $"An HTTP request error occurred: {ex.Message}");
                }
            }

            return responseBody;

            string? ExtractCPart(string URL)
            {
                var pattern = @"https://pu\.vk\.com/(\w+)/upload_doc\.php";
                var match = Regex.Match(URL, pattern);

                return match.Success ? match.Groups[1].Value : null;
            }

            string ExtractHash(string url)
            {
                var startIndex = url.IndexOf("hash=") + "hash=".Length;
                var endIndex = url.IndexOf("&", startIndex);

                return endIndex == -1 ? url[startIndex..] : url[startIndex..endIndex];
            }

            string ExtractRHash(string url)
            {
                var startIndex = url.IndexOf("rhash=") + "rhash=".Length;
                var endIndex = url.IndexOf("&", startIndex);

                return endIndex == -1 ? url.Substring(startIndex) : url.Substring(startIndex, endIndex - startIndex);
            }
        }

        private async Task<string> GetUploadURLAsync()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.vk.com/method/docs.getWallUploadServer");
            var s = settings;
            var content = new MultipartFormDataContent
            {
                { new StringContent(settings.AccessToken), "access_token" },
                { new StringContent(settings.GroupID.ToString()), "group_id" },
                { new StringContent(settings.ApiVersion), "v" }
            };

            request.Content = content;
            var response = await client.SendAsync(request);            
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var json = JsonObject.Parse(responseBody);
            var url = json["response"]["upload_url"].ToString();

            return url;
        }

        public static int CalculateChunkSize(string file, int maxChunkSizeMb)
        {
            var fileSizeBytes = new FileInfo(file).Length;
            var maxChunkSizeBytes = maxChunkSizeMb * 1024 * 1024;
            var numChunks = (int)Math.Ceiling((double)fileSizeBytes / maxChunkSizeBytes);
            var chunkSizeBytes = (int)Math.Ceiling((double)fileSizeBytes / numChunks);

            return (chunkSizeBytes / 1024 / 1024) + 1;
        }

        public void SplitFile(string fileToSplit, int chunkSizeMB, string sha256Checksum, string fileName)
        {
            var prefix = "0000";
            var bufferSize = 1024 * 1024 * chunkSizeMB;
            var index = 0;
            using var input = new FileStream(fileToSplit, FileMode.Open, FileAccess.Read);

            if (input.Length <= bufferSize)
            {
                var outputFile = Path.Combine($"{VKDriveFolder}", $"{sha256Checksum}", $"{fileName}-{prefix}{index + 1}.vkd");
                File.Copy(fileToSplit, outputFile);
                return;
            }

            var buffer = new byte[bufferSize];

            while (input.Position < input.Length)
            {
                var outputFile = Path.Combine($"{VKDriveFolder}", $"{sha256Checksum}", $"{fileName}-{prefix}{index + 1}.vkd");
                using var output = new FileStream(outputFile, FileMode.Create);
                var remaining = bufferSize;

                while (remaining > 0 && input.Position < input.Length)
                {
                    int bytesRead = input.Read(buffer, 0, Math.Min(remaining, bufferSize));
                    output.Write(buffer, 0, bytesRead);
                    remaining -= bytesRead;
                }

                index++;
            }
        }

        public void JoinParts(string partsPath, string assembledFilePath)
        {
            var notSorted = Directory.GetFiles(partsPath);
            var fileList = SortFiles(notSorted);
            using var outfile = new FileStream(assembledFilePath, FileMode.Create);
            foreach (var partFile in fileList)
            {
                using var infile = new FileStream(partFile, FileMode.Open);
                infile.CopyTo(outfile);
            }
        }

        private string[] SortFiles(string[] arr)
        {
            var sortedFiles = arr.OrderBy(f => int.Parse(Regex.Match(f, @"-(\d+)\.vkd$").Groups[1].Value));

            return sortedFiles.ToArray();
        }

        private static void DecompressFile(string archiveToDecompress, string outputFolder, string password)
        {
            using var zip = Ionic.Zip.ZipFile.Read(archiveToDecompress);
            zip.Password = password;
            zip.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
            try
            {
                zip.ExtractAll(outputFolder, ExtractExistingFileAction.OverwriteSilently);
            }
            catch (BadPasswordException)
            {
                ShowPopupErrorMessagebox("Ошибка", "Неверный пароль от архива. Возможно, файл был повреждён или изменён");
            }
            catch (IOException e)
            {
                ShowPopupErrorMessagebox("I/O exception", e.Message);
            }
        }

        private void CompressFile(string fileToCompress, string outputArchive, string password)
        {
            var level = GetCompressionLevel();
            using var zip = new Ionic.Zip.ZipFile()
            {
                UseZip64WhenSaving = Zip64Option.Always,
                Encryption = EncryptionAlgorithm.WinZipAes256,
                Password = password,
                CompressionLevel = level,
                AlternateEncoding = Encoding.UTF8,
                AlternateEncodingUsage = ZipOption.AsNecessary
            };

            zip.AddFile(fileToCompress, string.Empty);
            zip.Save(outputArchive);

            Ionic.Zlib.CompressionLevel GetCompressionLevel()
            {
                MyCompressionLevels selectedLevel = settings.CompressionLevel;

                return selectedLevel switch
                {
                    MyCompressionLevels.None => Ionic.Zlib.CompressionLevel.None,
                    MyCompressionLevels.Minimum => Ionic.Zlib.CompressionLevel.Level3,
                    MyCompressionLevels.Default => Ionic.Zlib.CompressionLevel.Default,
                    MyCompressionLevels.Best => Ionic.Zlib.CompressionLevel.BestCompression,
                    _ => throw new ArgumentException("Invalid compression level"),
                };
            }
        }

        private string GetFileSize(string file_path)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = new FileInfo(file_path).Length;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.

            return string.Format("{0:0.##} {1}", len, sizes[order]);
        }

        private static byte[] GenerateEncryptionKey(int size)
        {
            var key = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(key);

            return key;
        }

        private string EncryptFile(string filePath, byte[] key, byte[] iv)
        {
            var encryptedFilePath = Path.Combine(temporaryFolder, "EncryptedFile.vkl");
            using var aes = System.Security.Cryptography.Aes.Create();
            var tmp = settings;
            aes.Key = key;
            aes.IV = iv;

            using var inputFile = File.OpenRead(filePath);
            using var outputFile = File.Create(encryptedFilePath);
            using var encryptor = aes.CreateEncryptor();
            using var cryptoStream = new CryptoStream(outputFile, encryptor, CryptoStreamMode.Write);
            inputFile.CopyTo(cryptoStream);

            return encryptedFilePath;
        }

        private static void DecryptFile(string encryptedFilePath, byte[] key, byte[] initializationVector, string decryptedOutputFile)
        {
            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = key;
            aes.IV = initializationVector;

            Directory.CreateDirectory(Path.GetDirectoryName(decryptedOutputFile));

            using var inputFile = File.OpenRead(encryptedFilePath);
            using var outputFile = File.Create(decryptedOutputFile);
            using var decryptor = aes.CreateDecryptor();
            using var cryptoStream = new CryptoStream(inputFile, decryptor, CryptoStreamMode.Read);
            cryptoStream.CopyTo(outputFile);
        }

        /// <summary>
        /// Creates a JSON file with the specified parameters.
        /// </summary>        
        /// <param name="initializationVector">The initialization vector. An IV is used to ensure that encrypted data remains confidential 
        /// and secure, even if the same plaintext is encrypted multiple times. In essence, it makes sure that even if two plaintexts have 
        /// the same content, their corresponding ciphertexts will be different. The IV is usually included with the ciphertext in order 
        /// to allow the decryption process to reproduce the same encryption process.</param>
        /// <returns>The file path of the created JSON file.</returns>
        private string CreateJsonFile(string originalFileName, string fileSize, byte[] filePassword, byte[] initializationVector, string archivePassword, List<string> links, string jsonPath, string uniqueName)
        {
            var data = new
            {
                OriginalName = originalFileName,
                FileSize = fileSize,
                FilePassword = ByteToString(filePassword),
                InitializationVector = ByteToString(initializationVector),
                ArchivePassword = archivePassword,
                Links = links,
                UniqueName = uniqueName
            };

            var jsonOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true // optional: format the JSON with indentation
            };

            // Сериализовать объект в JSON строку
            string jsonString = System.Text.Json.JsonSerializer.Serialize(data, jsonOptions);

            if (!File.Exists(jsonPath))
            {
                try
                {
                    File.WriteAllText(jsonPath, jsonString);
                }
                catch (Exception ex)
                {
                    ShowPopupErrorMessagebox("Fail!", $"Failed to create file {jsonPath}: {ex.Message}");
                }
            }

            return jsonPath;
        }

        private string CreateSystemFolder(string sha256)
        {
            var path = Path.Combine(VKDriveFolder, sha256);

            // Check if the folder already exists
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);

                // Verify that the directory was created
                if (!Directory.Exists(path))
                {
                    throw new IOException($"Failed to create directory at {path}");
                }
            }

            return path;
        }

        public static string GetSHA256Checksum(string path)
        {
            try
            {
                using SHA256 mySHA256 = SHA256.Create();
                using FileStream fileStream = File.OpenRead(path);
                fileStream.Position = 0;
                var hashValue = mySHA256.ComputeHash(fileStream);

                return ByteArrayToString(hashValue);
            }
            catch (IOException e)
            {
                ShowPopupErrorMessagebox("Невозможно посчитать SHA256", e.Message);
                return $"I/O Exception: {e.Message}";
            }
            catch (UnauthorizedAccessException e)
            {
                ShowPopupErrorMessagebox("Невозможно посчитать SHA256", e.Message);
                return $"Access Exception: {e.Message}";
            }

            static string ByteArrayToString(byte[] array)
            {
                StringBuilder sb = new();

                for (int i = 0; i < array.Length; i++)
                {
                    sb.Append($"{array[i]:X2}");
                }

                return sb.ToString();
            }
        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All Files (*.*)|*.*"
            };
            var result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                var selectedFileName = openFileDialog.FileName;
                if (string.IsNullOrEmpty(settings.AccessToken))
                {
                    ShowPopupErrorMessagebox("Ошибка VK API", "Невозможно загрузить файл: не настроен токен доступа");
                    return;
                }

                await UploadFileToVKDriveAsync(selectedFileName);
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await RefreshFileList();
        }

        private void StartProgressBar()
        {
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
            timer1.Start();
        }

        private void StopProgressBar()
        {
            timer1.Stop();
            toolStripProgressBar1.Value = toolStripProgressBar1.Minimum;
            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedFile = (CloudFile)listBox1.SelectedItem;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (toolStripProgressBar1.Value < toolStripProgressBar1.Maximum)
            {
                toolStripProgressBar1.Value++;
            }
            else
            {
                toolStripProgressBar1.Value = toolStripProgressBar1.Minimum;
            }
        }

        private async void button_download_Click(object sender, EventArgs e)
        {
            await DownloadSelectedFileAsync();
        }

        private async void listBox1_DoubleClick(object sender, EventArgs e)
        {
            await DownloadSelectedFileAsync();
        }

        private async Task DownloadSelectedFileAsync()
        {
            if (listBox1.SelectedItem != null && _selectedFile != null)
            {
                StartProgressBar();
                DisableButtons();
                toolStripStatusLabel1.Text = $"Скачивание файла {_selectedFile.NameAndSize}";

                var filePartsToDownload = GetLinksFromJson(_selectedFile.jsonPath);
                var fileFolder = Path.Combine(VKDriveFolder, Path.GetFileNameWithoutExtension(_selectedFile.jsonPath));

                if (!Directory.Exists(fileFolder))
                {
                    Directory.CreateDirectory(fileFolder);
                }

                var counter = 0;
                foreach (var link in filePartsToDownload)
                {
                    var url = link.Trim();
                    using var client = new HttpClient();
                    using var response = await client.GetAsync(url);
                    using var content = response.Content;
                    var fileBytes = await content.ReadAsByteArrayAsync();
                    var prefix = "0000";
                    var fileName = $"To be assembled-{prefix}{counter + 1}.vkd";
                    var pathToBeWritten = Path.Combine(fileFolder, fileName);
                    await File.WriteAllBytesAsync(pathToBeWritten, fileBytes);
                    counter++;
                }

                var toBeDecrypted = Path.Combine(temporaryFolder, "ToBeDecrypted.tmp");
                var encryptedArchivePath = Path.Combine(temporaryFolder, "EncryptedArchive.zip");

                ChangeStatusbarText("Объединение частей файла");

                await Task.Run(() =>
                {
                    JoinParts(fileFolder, toBeDecrypted);
                });

                toolStripStatusLabel1.Text = "Рашифровка файла";

                var jsonText = File.ReadAllText(_selectedFile.jsonPath);
                dynamic jsonData = JsonConvert.DeserializeObject(jsonText);


                var originalFileName = jsonData["OriginalName"];
                await Task.Run(() =>
                {
                    var filePassword = StringToByte((string)jsonData["FilePassword"]);
                    var iv = StringToByte((string)jsonData["InitializationVector"]);
                    DecryptFile(toBeDecrypted, filePassword, iv, encryptedArchivePath);
                });

                toolStripStatusLabel1.Text = "Распаковка архива";
                await Task.Run(() =>
                {
                    DecompressFile(encryptedArchivePath, downloadedFolder, (string)jsonData["ArchivePassword"]);
                });

                toolStripStatusLabel1.Text = "Удаление временных файлов";
                ClearDirectory(fileFolder);
                ClearDirectory(temporaryFolder);
                EnableButtons();
                StopProgressBar();
                toolStripStatusLabel1.Text = statusbarLabelDefaultText;

                if (settings.OpenFolderAfterDownload)
                {
                    var path = Path.Combine(downloadedFolder, _selectedFile.Name);
                    var argument = "/select, \"" + path + "\"";
                    Process.Start("explorer.exe", argument);
                }
            }
        }

        private void ChangeStatusbarText(string text)
        {
            toolStripStatusLabel1.Text = text;
        }

        private void ClearDirectory(string directoryPath)
        {
            Directory.Delete(directoryPath, true);
            Directory.CreateDirectory(directoryPath);
        }

        private async void button_deleteFromCloud_Click_1(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                DisableButtons();
                await DeleteSelectedFileFromCloud();
                EnableButtons();
            }
        }

        private async Task DeleteSelectedFileFromCloud()
        {
            if (string.IsNullOrEmpty(settings.AccessToken))
            {
                ShowPopupErrorMessagebox("Ошибка VK API", "Невозможно удалить файл: не настроен токен доступа");
                return;
            }

            if (settings.AskBeforeDelete)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить этот файл?", "Удаление файла", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            var deleteStatus = $"Удаление файла {_selectedFile.NameAndSize}";
            toolStripStatusLabel1.Text = deleteStatus;
            var filePartsLinks = GetLinksFromJson(_selectedFile.jsonPath);
            var counter = 1;

            foreach (var link in filePartsLinks)
            {
                toolStripStatusLabel1.Text = $"{deleteStatus} {counter}/{filePartsLinks.Count}";
                await DeleteFilePartFromCloud(link);
                var rnd = new Random();
                await Task.Delay(rnd.Next(700));
                counter++;
            }

            File.Delete(_selectedFile.jsonPath);
            _cloudFiles.Remove(_selectedFile);
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };
            var jsonString = System.Text.Json.JsonSerializer.Serialize(_cloudFiles, jsonOptions);
            File.WriteAllText(_jsonCloudFilesLocation, jsonString);

            var folderPAth = Path.Combine(VKDriveFolder, Path.GetFileNameWithoutExtension(_selectedFile.jsonPath));

            if (Directory.Exists(folderPAth))
            {
                Directory.Delete(folderPAth, true);
            }

            if (settings.SoundsOn)
            {
                var audio = new SoundPlayer(VKDrive.Properties.Resources.recycle);
                audio.Play();
            }

            await RefreshFileList();
        }

        private async Task<string> DeleteFilePartFromCloud(string link)
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.vk.com/method/docs.delete");
            var ownerID = ExtractOwnerID(link);
            var docID = ExtractDocID(link);

            using var content = new MultipartFormDataContent
            {
                { new StringContent(ownerID), "owner_id" },
                { new StringContent(docID), "doc_id" },
                { new StringContent(settings.AccessToken), "access_token" },
                { new StringContent("5.131"), "v" }
            };
            request.Content = content;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        private string ExtractDocID(string link)
        {
            var docID = string.Empty;
            var startIndex = link.IndexOf("_");

            if (startIndex != -1)
            {
                var endIndex = link.IndexOf("?");

                if (endIndex != -1)
                {
                    var substring = link.Substring(startIndex + 1, endIndex - startIndex - 1);
                    docID = substring.Replace("_", "");
                }
            }

            return docID;
        }

        private string ExtractOwnerID(string link)
        {
            var ownerID = string.Empty;
            var startIndex = link.IndexOf("-");

            if (startIndex != -1)
            {
                var endIndex = link.IndexOf("_");

                if (endIndex != -1)
                {
                    ownerID = link.Substring(startIndex, endIndex - startIndex);
                }
            }

            return ownerID;
        }

        private async void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if (listBox1.SelectedItem != null)
                    {
                        DisableButtons();
                        await DeleteSelectedFileFromCloud();
                        EnableButtons();
                    }
                    break;
                case Keys.Enter:
                    if (listBox1.SelectedItem != null)
                    {
                        await DownloadSelectedFileAsync();
                    }
                    break;
            }
        }

        private async void settingsButton_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsWindow(settings);

            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                settings = settingsForm._settings;
                SetPriority();
                var jsonOptions = new JsonSerializerOptions()
                {
                    WriteIndented = true,
                };
                var jsonString = System.Text.Json.JsonSerializer.Serialize(settings, jsonOptions);
                File.WriteAllText(_jsonSettingsLocation, jsonString);
            }

            await RefreshFileList();
        }

        public static string GetXCharsOfString(string inputString, int numChars)
        {
            if (string.IsNullOrEmpty(inputString))
            {
                return string.Empty;
            }

            if (inputString.Length <= numChars)
            {
                return inputString;
            }

            return inputString[..numChars];
        }


        private void SetPriority()
        {
            Process.GetCurrentProcess().PriorityClass = settings.ProcessPriority;
            if (settings.ProcessPriority == ProcessPriorityClass.BelowNormal)
            {
                Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1);
            }
            else
            {
                var processorCount = Environment.ProcessorCount;
                var affinityMask = (IntPtr)((1 << processorCount) - 1);
                Process.GetCurrentProcess().ProcessorAffinity = affinityMask;
            }
        }

        public static void ShowPopupErrorMessagebox(string caption, string text)
        {
            MessageBox.Show(text, caption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            Show();
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (settings.ProcessPriority == ProcessPriorityClass.BelowNormal)
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    Hide();
                    notifyIcon1.Visible = true;
                    notifyIcon1.ShowBalloonTip(1000);
                }
                else if (WindowState == FormWindowState.Normal)
                {
                    notifyIcon1.Visible = false;
                }
            }
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void listBox1_DragLeave(object sender, EventArgs e)
        {
            listBox1.Invalidate();
        }

        private async void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            listBox1.Invalidate();
            Activate();
            var droppedFilePath = GetFilePath(e);

            await UploadFileToVKDriveAsync(droppedFilePath);
        }

        private string GetFilePath(DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);

            return file[0].ToString();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти?", "Подтвердите выход", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void toolStripProgressBar1_Click(object sender, EventArgs e)
        {
            _easterEggClicksCounter++;

            if (_easterEggClicksCounter == 10)
            {
                ShowEasterEgg();
                _easterEggClicksCounter = 0;
            }
        }

        private static void ShowEasterEgg()
        {
            MessageBox.Show("Congratulations! You found the Easter egg!", "Easter Egg", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}