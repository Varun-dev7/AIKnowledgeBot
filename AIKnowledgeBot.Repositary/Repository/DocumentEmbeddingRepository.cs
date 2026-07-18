using AIKnowledgeBot.InterFace.IRepository;
using AIKnowledgeBot.Models.Entities;
using AIKnowledgeBot.Repositary.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Repositary.Repository
{
    public class DocumentEmbeddingRepository : IDocumentEmbeddingRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentEmbeddingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(List<DocumentEmbedding> embeddings)
        {
            await _context.DocumentEmbeddings.AddRangeAsync(embeddings);
        }

        public async Task<List<DocumentEmbedding>> GetByDocumentIdAsync(Guid documentId)
        {
            return await _context.DocumentEmbeddings
                .Include(e => e.Chunk)
                .Where(e => e.Chunk.DocumentId == documentId)
                .ToListAsync();
        }

        public async Task<List<DocumentEmbedding>> GetAllAsync()
        {
            return await _context.DocumentEmbeddings
                .Include(e => e.Chunk)
                .ThenInclude(c => c.Document)
                .ToListAsync();
        }
    }
}
