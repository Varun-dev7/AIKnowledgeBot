using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIKnowledgeBot.Models.Entities;
namespace AIKnowledgeBot.InterFace.IRepository
{
    public interface IDocumentRepository
    {
        Task<Document> AddAsync(Document document);

        Task<Document?> GetByIdAsync(Guid id);

        Task<List<Document>> GetAllAsync();

        Task<bool> ExistsAsync(Guid id);

        Task UpdateAsync(Document document);

        Task DeleteAsync(Document document);

    }
}
