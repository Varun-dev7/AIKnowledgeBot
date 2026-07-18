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
    public class DocumentChunkRepository : IDocumentChunkRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentChunkRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(List<DocumentChunk> chunks)
        {
            await _context.DocumentChunks.AddRangeAsync(chunks);
        }

        public async Task<List<DocumentChunk>> GetByDocumentIdAsync(Guid documentId)
        {
            return await _context.DocumentChunks
                .Where(x => x.DocumentId == documentId)
                .OrderBy(x => x.ChunkIndex)
                .ToListAsync();
        }
    }
}
