using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;


namespace WinFormsApp1
{
    internal class Settings
    {
        private int _aesPasswordLength = 32;
        private int _chunkToUploadSize = 190; // Ограничение VK API - 200 MB

        public int ChunkToUploadSize 
        { 
            get => _chunkToUploadSize; 
            set
            {
                _chunkToUploadSize = Math.Min(value, 200);
            }
        }

        public int AesPasswordLength
        {
            get => _aesPasswordLength;
            set 
            { 
                _aesPasswordLength = Math.Min(value, 32); 
            }
        }

        public string AccessToken { get; set; } = string.Empty;

        public int HttpClientTimeout { get; set; } = 86400;

        public ProcessPriorityClass ProcessPriority { get; set; } = ProcessPriorityClass.Normal;
                
        public int ArchivePasswordLength { get; set; } = 5;

        public int GroupID { get; set; } = 39530977;

        public string ApiVersion { get; set; } = "5.131";

        public bool SoundsOn { get; set; }

        public bool AskBeforeDelete { get; set; } = true;

        public bool OpenFolderAfterDownload { get; set; } = true;   
        
        public bool SortByName { get;set; } = true;

        public bool SortByDate { get;set; }
    }
}
