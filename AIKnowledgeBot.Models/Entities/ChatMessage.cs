using AIKnowledgeBot.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Models.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; set; }

        public Guid ConversationId { get; set; }

        public Conversation Conversation { get; set; }

        public ChatRole Role { get; set; }

        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
