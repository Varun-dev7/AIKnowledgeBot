using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Models.DTOs
{
    public class ChatResponseDto
    {
        public Guid ConversationId { get; set; }
        public string Answer { get; set; } = string.Empty;

        public List<SourceDto> Sources { get; set; } = new();
    }
}
