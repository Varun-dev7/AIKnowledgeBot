using AIKnowledgeBot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.InterFace.IRepository
{
    public interface IChatMessageRepository
    {
        Task<List<ChatMessage>> GetByConversationIdAsync(Guid conversationId);

        Task AddAsync(ChatMessage message);

        Task AddRangeAsync(IEnumerable<ChatMessage> messages);
    }
}
