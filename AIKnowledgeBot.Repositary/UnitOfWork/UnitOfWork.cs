using AIKnowledgeBot.InterFace.IRepository;
using AIKnowledgeBot.Models.Entities;
using AIKnowledgeBot.Repositary.Context;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Repositary.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IDbContextTransaction? _transaction;

        public IDocumentRepository Documents { get; }
        public ICategoryRepository Categories { get; }

        public ISubjectRepository Subjects { get; }
        public IDocumentChunkRepository Chunks { get; }
        public IDocumentEmbeddingRepository Embeddings { get; }
        public IConversationRepository Conversations { get; }

        public IChatMessageRepository ChatMessages { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IDocumentRepository documentRepository,
            ICategoryRepository categoryRepository,
            ISubjectRepository subjectRepository,
            IDocumentChunkRepository chunkRepository,
            IDocumentEmbeddingRepository embeddingRepository,
            IConversationRepository conversations,
            IChatMessageRepository chatMessages)
        {
            _context = context;
            Documents = documentRepository;
            Categories = categoryRepository;
            Subjects = subjectRepository;
            Chunks = chunkRepository;
            Embeddings = embeddingRepository;
            Conversations = conversations;
            ChatMessages = chatMessages;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
