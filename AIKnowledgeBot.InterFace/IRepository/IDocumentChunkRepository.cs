using AIKnowledgeBot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.InterFace.IRepository
{
    public interface IDocumentChunkRepository
    {
        Task AddRangeAsync(List<DocumentChunk> chunks);

        Task<List<DocumentChunk>> GetByDocumentIdAsync(Guid documentId);
    }
}
