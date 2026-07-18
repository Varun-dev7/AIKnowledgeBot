using AIKnowledgeBot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.InterFace.IRepository
{
    public interface IDocumentEmbeddingRepository
    {
        Task AddRangeAsync(List<DocumentEmbedding> embeddings);

        Task<List<DocumentEmbedding>> GetByDocumentIdAsync(Guid documentId);
        Task<List<DocumentEmbedding>> GetAllAsync();
    }
}
