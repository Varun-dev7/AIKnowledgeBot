using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Models.DTOs.Document
{
    public class DocumentResponseDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string OriginalFileName { get; set; } = string.Empty;

        public string StoredFileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime UploadedDate { get; set; }

        public int CategoryId { get; set; }

        public int SubjectId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string SubjectName { get; set; } = string.Empty;
    }
}
