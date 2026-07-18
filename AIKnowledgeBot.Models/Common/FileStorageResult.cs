using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Models.Common
{
    public class FileStorageResult
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public string StoredFileName { get; set; } = string.Empty;

        public string OriginalFileName { get; set; } = string.Empty;

        public string RelativePath { get; set; } = string.Empty;

        public string Extension { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }
    }
}
