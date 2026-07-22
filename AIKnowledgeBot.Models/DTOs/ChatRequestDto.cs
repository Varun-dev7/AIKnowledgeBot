using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Models.DTOs
{
    public class ChatRequestDto
    {
        public Guid? ConversationId { get; set; }
        [Required]
        public string Question { get; set; } = string.Empty;
    }
}
