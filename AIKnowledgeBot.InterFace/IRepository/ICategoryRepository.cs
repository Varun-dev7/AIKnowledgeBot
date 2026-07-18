using AIKnowledgeBot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.InterFace.IRepository
{
    public interface ICategoryRepository
    {
        Task<bool> ExistsAsync(int id);

        Task<Category?> GetByIdAsync(int id);

        Task<List<Category>> GetAllAsync();
    }
}
