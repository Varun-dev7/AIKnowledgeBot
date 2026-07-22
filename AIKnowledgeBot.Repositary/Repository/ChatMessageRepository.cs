using AIKnowledgeBot.InterFace.IRepository;
using AIKnowledgeBot.Repositary.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AIKnowledgeBot.Models.Entities;

namespace AIKnowledgeBot.Repositary.Repository
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatMessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatMessage>> GetByConversationIdAsync(Guid conversationId)
        {
            return await _context.ChatMessages
                .Where(x => x.ConversationId == conversationId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(ChatMessage message)
        {
            await _context.ChatMessages.AddAsync(message);
        }

        public async Task AddRangeAsync(IEnumerable<ChatMessage> messages)
        {
            await _context.ChatMessages.AddRangeAsync(messages);
        }
    }
}
