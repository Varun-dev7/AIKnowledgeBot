using AIKnowledgeBot.InterFace.IRepository;
using AIKnowledgeBot.Models.Entities;
using AIKnowledgeBot.Repositary.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AIKnowledgeBot.Repositary.Repository
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly ApplicationDbContext _context;

        public ConversationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Conversation?> GetByIdAsync(Guid id)
        {
            return await _context.Conversations
                .Include(x => x.Messages)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Conversation>> GetAllAsync()
        {
            return await _context.Conversations
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Conversation conversation)
        {
            await _context.Conversations.AddAsync(conversation);
        }

        public Task UpdateAsync(Conversation conversation)
        {
            _context.Conversations.Update(conversation);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Conversation conversation)
        {
            _context.Conversations.Remove(conversation);
            return Task.CompletedTask;
        }
    }
}
