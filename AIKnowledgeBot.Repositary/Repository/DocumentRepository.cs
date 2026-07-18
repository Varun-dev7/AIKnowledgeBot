using AIKnowledgeBot.Repositary.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIKnowledgeBot.InterFace.IRepository;
using AIKnowledgeBot.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIKnowledgeBot.Repositary.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Document> AddAsync(Document document)
        {
            await _context.Documents.AddAsync(document);
            return document;
        }

        public async Task<Document?> GetByIdAsync(Guid id)
        {
            return await _context.Documents
                .Include(x => x.Category)
                .Include(x => x.Subject)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Document>> GetAllAsync()
        {
            return await _context.Documents
                .Include(x => x.Category)
                .Include(x => x.Subject)
                .OrderByDescending(x => x.UploadedDate)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Documents
                .AnyAsync(x => x.Id == id);
        }

        public Task UpdateAsync(Document document)
        {
            _context.Documents.Update(document);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Document document)
        {
            _context.Documents.Remove(document);
            return Task.CompletedTask;
        }
    }
}
