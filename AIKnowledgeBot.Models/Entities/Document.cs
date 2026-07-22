using AIKnowledgeBot.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Models.Entities
{
    public class Document
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category? Category { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual Subject? Subject { get; set; }

        [Required]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(255)]
        public string OriginalFileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string StoredFileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Extension { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [MaxLength(150)]
        public string UploadedBy { get; set; } = "Admin";

        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastProcessedDate { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<DocumentChunk> Chunks { get; set; }= new List<DocumentChunk>();

        public DocumentStatus Status { get; set; } = DocumentStatus.Pending;

        public int TotalChunks { get; set; }

        public int ProcessedChunks { get; set; }

        public string? ProcessingMessage { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}