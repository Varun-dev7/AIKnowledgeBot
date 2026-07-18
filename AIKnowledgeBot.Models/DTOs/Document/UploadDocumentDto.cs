using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AIKnowledgeBot.Models.DTOs.Document
{
    public class UploadDocumentDto
    {
        [Required]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public IFormFile File { get; set; } = default!;
    }
}
