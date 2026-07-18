using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Models.Entities
{
    public class DocumentChunk
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid DocumentId { get; set; }

        [ForeignKey(nameof(DocumentId))]
        public virtual Document? Document { get; set; }

        [Required]
        public int PageNumber { get; set; }

        [Required]
        public int ChunkIndex { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public virtual DocumentEmbedding? Embedding { get; set; }
    }
}
