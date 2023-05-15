using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    internal class CloudFile
    {
        public string Name { get; set; } = string.Empty;

        public string UniqueName { get; set; } = string.Empty;

        public string jsonPath { get; set; } = string.Empty;

        public DateTime CreationDate { get; set; }

        public string Size { get; set; } = string.Empty;

        public string NameAndSize => $"{Name} - {Size}";

        public bool IsFolder { get; set; }
    }
}
