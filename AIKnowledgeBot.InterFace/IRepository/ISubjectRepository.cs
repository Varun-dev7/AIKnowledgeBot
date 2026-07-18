using AIKnowledgeBot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.InterFace.IRepository
{
    public interface ISubjectRepository
    {
        Task<bool> ExistsAsync(int id);

        Task<Subject?> GetByIdAsync(int id);

        Task<List<Subject>> GetAllAsync();
    }
}
