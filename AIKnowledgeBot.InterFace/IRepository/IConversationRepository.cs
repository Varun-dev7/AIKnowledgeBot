using AIKnowledgeBot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.InterFace.IRepository
{
    public interface IConversationRepository
    {
        Task<Conversation?> GetByIdAsync(Guid id);

        Task<List<Conversation>> GetAllAsync();

        Task AddAsync(Conversation conversation);

        Task UpdateAsync(Conversation conversation);

        Task DeleteAsync(Conversation conversation);
    }
}
